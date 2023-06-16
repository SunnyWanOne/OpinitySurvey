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
    public class SurveyService : ServiceBase, ISurveyService, IService
    {
        private readonly SiteState _siteState;

        public SurveyService(HttpClient http, SiteState siteState) : base(http, siteState) 
        {
            _siteState = siteState;
        }

        private string Apiurl => CreateApiUrl("Survey", _siteState.Alias);

        public async Task<List<Models.OqtaneSurvey>> GetSurveysAsync(int ModuleId)
        {
            List<Models.OqtaneSurvey> Surveys = await GetJsonAsync<List<Models.OqtaneSurvey>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId));
            return Surveys.OrderBy(item => item.SurveyName).ToList();
        }

        public async Task<Models.OqtaneSurvey> GetSurveyAsync(int ModuleId)
        {
            return await GetJsonAsync<Models.OqtaneSurvey>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ModuleId}", EntityNames.Module, ModuleId));
        }

        public async Task<Models.OqtaneSurvey> AddSurveyAsync(Models.OqtaneSurvey Survey)
        {
            return await PostJsonAsync<Models.OqtaneSurvey>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, Survey.ModuleId), Survey);
        }

        public async Task<Models.OqtaneSurvey> UpdateSurveyAsync(Models.OqtaneSurvey Survey)
        {
            return await PutJsonAsync<Models.OqtaneSurvey>(CreateAuthorizationPolicyUrl($"{Apiurl}/{Survey.SurveyId}", EntityNames.Module, Survey.ModuleId), Survey);
        }

        public async Task DeleteSurveyAsync(int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{ModuleId}", EntityNames.Module, ModuleId));
        }
    }
}
