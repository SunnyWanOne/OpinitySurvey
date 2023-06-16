using Opinity.Survey.Models;
using Oqtane.Modules;
using Oqtane.Services;
using Oqtane.Shared;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Opinity.Survey.Services
{
    public class SurveyAnswersService : ServiceBase, ISurveyAnswersService, IService
    {
        private readonly SiteState _siteState;

        public SurveyAnswersService(HttpClient http, SiteState siteState) : base(http, siteState)
        {
            _siteState = siteState;
        }

        private string Apiurl => CreateApiUrl("SurveyAnswers", _siteState.Alias);

        public async Task<List<Models.OqtaneSurveyItem>> SurveyResultsDataAsync(int ModuleId, int SelectedSurveyId, LoadDataArgs args)
        {
            return await PostJsonAsync<LoadDataArgs, List<Models.OqtaneSurveyItem>>(
                CreateAuthorizationPolicyUrl($"{Apiurl}/{SelectedSurveyId}", EntityNames.Module, ModuleId),
                args);
        }

        public async Task CreateSurveyAnswersAsync(Models.OqtaneSurvey Survey)
        {
            await PostJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, Survey.ModuleId), Survey);
        }
    }
}
