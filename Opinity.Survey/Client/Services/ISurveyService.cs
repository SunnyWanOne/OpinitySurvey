using System.Collections.Generic;
using System.Threading.Tasks;
using Opinity.Survey.Models;

namespace Opinity.Survey.Services
{
    public interface ISurveyService 
    {
        Task<List<Models.OqtaneSurvey>> GetSurveysAsync(int ModuleId);

        Task<Models.OqtaneSurvey> GetSurveyAsync(int ModuleId);

        Task<Models.OqtaneSurvey> AddSurveyAsync(Models.OqtaneSurvey Survey);

        Task<Models.OqtaneSurvey> UpdateSurveyAsync(Models.OqtaneSurvey Survey);

        Task DeleteSurveyAsync(int ModuleId);
    }
}
