using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opinity.Survey.Models;
using Opinity.Survey.Repository;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using Oqtane.Shared;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opinity.Survey.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class SurveyAnswersController : ModuleControllerBase
    {
        private readonly ISurveyRepository _SurveyRepository;
        private readonly IUserRepository _users;

        public SurveyAnswersController(ISurveyRepository SurveyRepository, IUserRepository users, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _SurveyRepository = SurveyRepository;
            _users = users;
        }

        // POST api/<controller>/1
        [HttpPost("{SelectedSurveyId}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public List<Models.OqtaneSurveyItem> Post(int SelectedSurveyId, [FromBody] LoadDataArgs args)
        {
            List<Models.OqtaneSurveyItem> Response = new List<Models.OqtaneSurveyItem>();

            Response = _SurveyRepository.SurveyResultsData(SelectedSurveyId, args);

            return Response;
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public void Post([FromBody] Models.OqtaneSurvey Survey)
        {
            if (ModelState.IsValid && Survey.ModuleId == _authEntityId[EntityNames.Module])
            {
                // Get User
                if (this.User.Identity.IsAuthenticated)
                {
                    var User = _users.GetUser(this.User.Identity.Name);

                    // Add User to Survey object
                    Survey.UserId = User.UserId;
                }
                else
                {
                    // The AnonymousCookie was passed by the Client
                    Survey.UserId = null;
                }

                bool boolResult = _SurveyRepository.CreateSurveyAnswers(Survey);

                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Survey Answers Added {Survey}", Survey);
            }
        }
        private IEnumerable<Models.OqtaneSurveyItem> ConvertToSurveyItems(List<OqtaneSurveyItem> colOqtaneSurveyItems)
        {
            List<Models.OqtaneSurveyItem> colSurveyItemCollection = new List<Models.OqtaneSurveyItem>();

            foreach (var objOqtaneSurveyItem in colOqtaneSurveyItems)
            {
                // Convert to SurveyItem
                Models.OqtaneSurveyItem objAddSurveyItem = ConvertToSurveyItem(objOqtaneSurveyItem);

                // Add to Collection
                colSurveyItemCollection.Add(objAddSurveyItem);
            }

            return colSurveyItemCollection;
        }
        private Models.OqtaneSurveyItem ConvertToSurveyItem(OqtaneSurveyItem objOqtaneSurveyItem)
        {
            if (objOqtaneSurveyItem == null)
            {
                return new Models.OqtaneSurveyItem();
            }

            // Create new Object
            Models.OqtaneSurveyItem objSurveyItem = new OqtaneSurveyItem();

            objSurveyItem.Id = objOqtaneSurveyItem.Id;
            objSurveyItem.ItemLabel = objOqtaneSurveyItem.ItemLabel;
            objSurveyItem.ItemType = objOqtaneSurveyItem.ItemType;
            objSurveyItem.ItemValue = objOqtaneSurveyItem.ItemValue;
            objSurveyItem.Position = objOqtaneSurveyItem.Position;
            objSurveyItem.Required = objOqtaneSurveyItem.Required;
            objSurveyItem.SurveyChoiceId = objOqtaneSurveyItem.SurveyChoiceId;

            // Create new Collection
            objSurveyItem.SurveyItemOption = new List<OqtaneSurveyItemOption>();

            foreach (OqtaneSurveyItemOption objOqtaneSurveyItemOption in objOqtaneSurveyItem.OqtaneSurveyItemOption)
            {
                // Create new Object
                Models.OqtaneSurveyItemOption objAddSurveyItemOption = new OqtaneSurveyItemOption();

                objAddSurveyItemOption.Id = objOqtaneSurveyItemOption.Id;
                objAddSurveyItemOption.OptionLabel = objOqtaneSurveyItemOption.OptionLabel;

                // Add to Collection
                objSurveyItem.SurveyItemOption.Add(objAddSurveyItemOption);
            }

            return objSurveyItem;
        }
    }
}
