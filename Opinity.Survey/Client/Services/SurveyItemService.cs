using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using Opinity.Survey.Models;

namespace Opinity.Survey.Services
{
    public class SurveyItemService : ServiceBase, ISurveyItemService, IService
    {
        private readonly SiteState _siteState;

        public SurveyItemService(HttpClient http, SiteState siteState) : base(http, siteState)
        {
            _siteState = siteState;
        }

        private string Apiurl => CreateApiUrl("SurveyItem", _siteState.Alias);

        public async Task<List<Models.OqtaneSurveyItem>> GetSurveyItemsAsync(int ModuleId)
        {
            List<Models.OqtaneSurveyItem> Surveys = await GetJsonAsync<List<Models.OqtaneSurveyItem>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId));
            return Surveys.OrderBy(item => item.Position).ToList();
        }

        public async Task<Models.OqtaneSurveyItem> GetSurveyItemAsync(int ModuleId)
        {
            return await GetJsonAsync<Models.OqtaneSurveyItem>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<Models.OqtaneSurveyItem> AddSurveyItemAsync(Models.OqtaneSurveyItem SurveyItem)
        {
            return await PostJsonAsync<Models.OqtaneSurveyItem>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, SurveyItem.ModuleId), SurveyItem);
        }

        public async Task<Models.OqtaneSurveyItem> MoveSurveyItemAsync(string MoveType, Models.OqtaneSurveyItem SurveyItem)
        {
            return await PostJsonAsync<Models.OqtaneSurveyItem>(CreateAuthorizationPolicyUrl($"{Apiurl}/{MoveType}", EntityNames.Module, SurveyItem.ModuleId), SurveyItem);
        }

        public async Task<Models.OqtaneSurveyItem> UpdateSurveyItemAsync(Models.OqtaneSurveyItem SurveyItem)
        {
            return await PutJsonAsync<Models.OqtaneSurveyItem>(CreateAuthorizationPolicyUrl($"{Apiurl}/{SurveyItem.Id}", EntityNames.Module, SurveyItem.ModuleId), SurveyItem);
        }

        public async Task DeleteSurveyItemAsync(Models.OqtaneSurveyItem SurveyItem)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{SurveyItem.Id}", EntityNames.Module, SurveyItem.ModuleId));
        }
    }
}
