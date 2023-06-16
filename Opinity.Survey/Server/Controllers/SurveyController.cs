using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Opinity.Survey.Repository;
using Oqtane.Controllers;
using System.Net;
using Oqtane.Repository;
using Opinity.Survey.Models;

namespace Opinity.Survey.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class SurveyController : ModuleControllerBase
    {
        private readonly ISurveyRepository _SurveyRepository;
        private readonly IUserRepository _users;

        public SurveyController(ISurveyRepository SurveyRepository, IUserRepository users, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _SurveyRepository = SurveyRepository;
            _users = users;
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public IEnumerable<Models.OqtaneSurvey> Get(string moduleid)
        {
            var colSurveys = _SurveyRepository.GetAllSurveysByModule(int.Parse(moduleid));
            return ConvertToSurveys(colSurveys);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public Models.OqtaneSurvey Get(int id)
        {
            var objSurvey = _SurveyRepository.GetSurvey(id);

            Models.OqtaneSurvey Survey = ConvertToSurvey(objSurvey);

            if (Survey != null && Survey.ModuleId != _authEntityId[EntityNames.Module])
            {
                Survey = null;
            }

            return Survey;
        }


        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public Models.OqtaneSurvey Post([FromBody] Models.OqtaneSurvey Survey)
        {
            if (ModelState.IsValid && Survey.ModuleId == _authEntityId[EntityNames.Module])
            {
                // Get User
                var User = _users.GetUser(this.User.Identity.Name);

                // Add User to Survey object
                Survey.UserId = User.UserId;

                Survey = ConvertToSurvey(_SurveyRepository.CreateSurvey(Survey));
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "Survey Added {Survey}", Survey);
            }
            return Survey;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public Models.OqtaneSurvey Put(int id, [FromBody] Models.OqtaneSurvey Survey)
        {
            if (ModelState.IsValid && Survey.ModuleId == _authEntityId[EntityNames.Module])
            {
                Survey = ConvertToSurvey(_SurveyRepository.UpdateSurvey(Survey));
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "Survey Updated {Survey}", Survey);
            }
            return Survey;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public void Delete(int id)
        {
            var objSurvey = _SurveyRepository.GetSurvey(id);

            Models.OqtaneSurvey Survey = ConvertToSurvey(objSurvey);

            if (Survey != null && Survey.ModuleId == _authEntityId[EntityNames.Module])
            {
                // Delete all Survey Items
                if (Survey.SurveyItem != null)
                {
                    foreach (var item in Survey.SurveyItem)
                    {
                        bool boolDeleteSurveyItemResult = _SurveyRepository.DeleteSurveyItem(item.Id);

                        if (boolDeleteSurveyItemResult)
                        {
                            _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Survey Item Deleted {item.Id}", item.Id);
                        }
                        else
                        {
                            _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Error: Survey Item *NOT* Deleted {item.Id}", item.Id);
                        }
                    }
                }

                bool boolResult = _SurveyRepository.DeleteSurvey(id);

                if (boolResult)
                {
                    _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Survey Deleted {id}", id);
                }
                else
                {
                    _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Error: Survey *NOT* Deleted {id}", id);
                }
            }
        }

        // Utility
        #region private IEnumerable<Models.OqtaneSurvey> ConvertToSurveys(List<OqtaneSurvey> colOqtaneSurveys)
        private IEnumerable<Models.OqtaneSurvey> ConvertToSurveys(List<OqtaneSurvey> colOqtaneSurveys)
        {
            List<Models.OqtaneSurvey> colSurveyCollection = new List<Models.OqtaneSurvey>();

            foreach (var objOqtaneSurvey in colOqtaneSurveys)
            {
                // Convert to Survey
                Models.OqtaneSurvey objAddSurvey = ConvertToSurvey(objOqtaneSurvey);

                // Add to Collection
                colSurveyCollection.Add(objAddSurvey);
            }

            return colSurveyCollection;
        }
        #endregion

        #region private static Models.Survey ConvertToSurvey(OqtaneSurvey objOqtaneSurvey)
        private Models.OqtaneSurvey ConvertToSurvey(OqtaneSurvey objOqtaneSurvey)
        {
            if (objOqtaneSurvey == null)
            {
                return new Models.OqtaneSurvey();
            }

            // Create new Object
            Models.OqtaneSurvey objAddSurvey = new Models.OqtaneSurvey();

            objAddSurvey.SurveyId = objOqtaneSurvey.SurveyId;
            objAddSurvey.ModuleId = objOqtaneSurvey.ModuleId;
            objAddSurvey.SurveyName = objOqtaneSurvey.SurveyName;
            objAddSurvey.CreatedBy = objOqtaneSurvey.CreatedBy;
            objAddSurvey.CreatedOn = objOqtaneSurvey.CreatedOn;
            objAddSurvey.ModifiedBy = objOqtaneSurvey.ModifiedBy;
            objAddSurvey.ModifiedOn = objOqtaneSurvey.ModifiedOn;
            if (objOqtaneSurvey.UserId != null)
            {
                objAddSurvey.UserId = objOqtaneSurvey.UserId.Value;
            }

            // Create new Collection
            objAddSurvey.SurveyItem = new List<OqtaneSurveyItem>();

            foreach (OqtaneSurveyItem objOqtaneSurveyItem in objOqtaneSurvey.OqtaneSurveyItem)
            {
                // Create new Object
                Models.OqtaneSurveyItem objAddSurveyItem = new OqtaneSurveyItem();

                objAddSurveyItem.Id = objOqtaneSurveyItem.Id;
                objAddSurveyItem.ItemLabel = objOqtaneSurveyItem.ItemLabel;
                objAddSurveyItem.ItemType = objOqtaneSurveyItem.ItemType;
                objAddSurveyItem.ItemValue = objOqtaneSurveyItem.ItemValue;
                objAddSurveyItem.Position = objOqtaneSurveyItem.Position;
                objAddSurveyItem.Required = objOqtaneSurveyItem.Required;
                objAddSurveyItem.SurveyChoiceId = objOqtaneSurveyItem.SurveyChoiceId;

                // Create new Collection
                objAddSurveyItem.SurveyItemOption = new List<OqtaneSurveyItemOption>();

                foreach (OqtaneSurveyItemOption objOqtaneSurveyItemOption in objOqtaneSurveyItem.OqtaneSurveyItemOption)
                {
                    // Create new Object
                    Models.OqtaneSurveyItemOption objAddSurveyItemOption = new OqtaneSurveyItemOption();

                    objAddSurveyItemOption.Id = objOqtaneSurveyItemOption.Id;
                    objAddSurveyItemOption.OptionLabel = objOqtaneSurveyItemOption.OptionLabel;

                    // Add to Collection
                    objAddSurveyItem.SurveyItemOption.Add(objAddSurveyItemOption);
                }

                // Add to Collection
                objAddSurvey.SurveyItem.Add(objAddSurveyItem);
            }

            return objAddSurvey;
        }
        #endregion
    }
}