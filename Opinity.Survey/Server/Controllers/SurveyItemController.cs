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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opinity.Survey.Controllers
{
    public class SurveyItemController : ModuleControllerBase
    {
        private readonly ISurveyRepository _SurveyRepository;
        private readonly IUserRepository _users;

        public SurveyItemController(ISurveyRepository SurveyRepository, IUserRepository users, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _SurveyRepository = SurveyRepository;
            _users = users;
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public IEnumerable<Models.OqtaneSurveyItem> Get(string moduleid)
        {
            var colSurveyItems = _SurveyRepository.GetAllSurveyItems(int.Parse(moduleid));
            return ConvertToSurveyItems(colSurveyItems);
        }

        // GET api/<controller>?/5
        [HttpGet("{id}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public Models.OqtaneSurveyItem Get(int id)
        {
            var objSurvey = _SurveyRepository.GetSurveyItem(id);

            Models.OqtaneSurveyItem SurveyItem = ConvertToSurveyItem(objSurvey);

            return SurveyItem;
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Policy = PolicyNames.EditModule)]
        public Models.OqtaneSurveyItem Post([FromBody] Models.OqtaneSurveyItem SurveyItem)
        {
            if (ModelState.IsValid && SurveyItem.ModuleId == _authEntityId[EntityNames.Module])
            {
                SurveyItem = ConvertToSurveyItem(_SurveyRepository.CreateSurveyItem(SurveyItem));
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "SurveyItem Added {SurveyItem}", SurveyItem);
            }
            return SurveyItem;
        }

        // POST api/<controller>/Down
        [HttpPost("{MoveType}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public Models.OqtaneSurveyItem Post(string MoveType, [FromBody] Models.OqtaneSurveyItem SurveyItem)
        {
            if (ModelState.IsValid && SurveyItem.ModuleId == _authEntityId[EntityNames.Module])
            {
                // Get the Survey (and all SurveyItems)
                var objSurvey = _SurveyRepository.GetSurvey(SurveyItem.ModuleId);

                if (MoveType == "Up")
                {
                    // Move Up
                    int DesiredPosition = (SurveyItem.Position - 1);

                    // Move the current element in that position
                    var CurrentSurveyItem =
                            objSurvey.OqtaneSurveyItem
                            .Where(x => x.Position == DesiredPosition)
                            .FirstOrDefault();

                    if (CurrentSurveyItem != null)
                    {
                        // Move it down
                        CurrentSurveyItem.Position = CurrentSurveyItem.Position + 1;

                        // Update it
                        _SurveyRepository.UpdateSurveyItem(ConvertToSurveyItem(CurrentSurveyItem));
                    }

                    // Move Item Up
                    var SurveyItemToMoveUp =
                         objSurvey.OqtaneSurveyItem
                            .Where(x => x.Id == SurveyItem.Id)
                            .FirstOrDefault();

                    if (SurveyItemToMoveUp != null)
                    {
                        // Move it up
                        SurveyItemToMoveUp.Position = SurveyItemToMoveUp.Position - 1;

                        // Update it
                        _SurveyRepository.UpdateSurveyItem(ConvertToSurveyItem(SurveyItemToMoveUp));
                    }
                }
                else
                {
                    // Move Down

                    int DesiredPosition = (SurveyItem.Position + 1);

                    // Move the current element in that position
                    var CurrentSurveyItem =
                            objSurvey.OqtaneSurveyItem
                            .Where(x => x.Position == DesiredPosition)
                            .FirstOrDefault();

                    if (CurrentSurveyItem != null)
                    {
                        // Move it down
                        CurrentSurveyItem.Position = CurrentSurveyItem.Position - 1;

                        // Update it
                        _SurveyRepository.UpdateSurveyItem(ConvertToSurveyItem(CurrentSurveyItem));
                    }

                    // Move Item Up
                    var SurveyItemToMoveUp =
                         objSurvey.OqtaneSurveyItem
                            .Where(x => x.Id == SurveyItem.Id)
                            .FirstOrDefault();

                    if (SurveyItemToMoveUp != null)
                    {
                        // Move it up
                        SurveyItemToMoveUp.Position = SurveyItemToMoveUp.Position + 1;

                        // Update it
                        _SurveyRepository.UpdateSurveyItem(ConvertToSurveyItem(SurveyItemToMoveUp));
                    }

                }

                _logger.Log(LogLevel.Information, this, LogFunction.Create, "SurveyItem {SurveyItem} moved", SurveyItem);
            }
            return SurveyItem;
        }
        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public Models.OqtaneSurveyItem Put(int id, [FromBody] Models.OqtaneSurveyItem SurveyItem)
        {
            if (ModelState.IsValid && SurveyItem.ModuleId == _authEntityId[EntityNames.Module])
            {
                SurveyItem = ConvertToSurveyItem(_SurveyRepository.UpdateSurveyItem(SurveyItem));
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "SurveyItem Updated {SurveyItem}", SurveyItem);
            }
            return SurveyItem;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public void Delete(int id)
        {
            var objSurveyItem = _SurveyRepository.GetSurveyItem(id);

            if (objSurveyItem != null)
            {
                bool boolResult = _SurveyRepository.DeleteSurveyItem(id);
                if (boolResult)
                {
                    _logger.Log(LogLevel.Information, this, LogFunction.Delete, "SurveyItem Deleted {Id}", id);
                }
                else
                {
                    _logger.Log(LogLevel.Information, this, LogFunction.Delete, "Error: SurveyItem *NOT* Deleted {Id}", id);
                }
            }
        }

        // Utility

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
