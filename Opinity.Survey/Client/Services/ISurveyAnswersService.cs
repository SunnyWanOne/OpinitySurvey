using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opinity.Survey.Services
{
    public interface ISurveyAnswersService
    {
        Task<List<Models.OqtaneSurveyItem>> SurveyResultsDataAsync(int ModuleId, int SelectedSurveyId, LoadDataArgs args);
        Task CreateSurveyAnswersAsync(Models.OqtaneSurvey Survey);
    }
}
