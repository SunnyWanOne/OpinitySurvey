using System.Collections.Generic;
using System.Threading.Tasks;
using Opinity.Survey.Models;
using Opinity.Survey.Repository;
using Radzen;

namespace Opinity.Survey.Repository
{
    public interface ISurveyRepository
    {
        Task<List<OqtaneSurvey>> GetAllSurveysAsync();
        List<OqtaneSurvey> GetAllSurveysByModule(int ModuleId);
        OqtaneSurvey GetSurvey(int Id);
        OqtaneSurvey CreateSurvey(Models.OqtaneSurvey NewSurvey);
        OqtaneSurvey UpdateSurvey(Models.OqtaneSurvey objExistingSurvey);
        bool DeleteSurvey(int id);
        List<OqtaneSurveyItem> GetAllSurveyItems(int ModuleId);
        OqtaneSurveyItem GetSurveyItem(int SurveyItemId);
        OqtaneSurveyItem CreateSurveyItem(Models.OqtaneSurveyItem NewSurveyItem);
        OqtaneSurveyItem UpdateSurveyItem(Models.OqtaneSurveyItem objExistingSurveyItem);
        bool DeleteSurveyItem(int Id);
        bool CreateSurveyAnswers(Models.OqtaneSurvey paramDTOSurvey);
        List<OqtaneSurveyItem> SurveyResultsData(int SelectedSurveyId, LoadDataArgs args);
    }
}
