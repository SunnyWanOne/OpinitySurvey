using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opinity.Survey.Services
{
    public interface ISurveyItemService
    {
        Task<List<Models.OqtaneSurveyItem>> GetSurveyItemsAsync(int ModuleId);

        Task<Models.OqtaneSurveyItem> GetSurveyItemAsync(int ModuleId);

        Task<Models.OqtaneSurveyItem> AddSurveyItemAsync(Models.OqtaneSurveyItem SurveyItem);

        Task<Models.OqtaneSurveyItem> MoveSurveyItemAsync(string MoveType, Models.OqtaneSurveyItem SurveyItem);

        Task<Models.OqtaneSurveyItem> UpdateSurveyItemAsync(Models.OqtaneSurveyItem SurveyItem);

        Task DeleteSurveyItemAsync(Models.OqtaneSurveyItem SurveyItem);
    }
}
