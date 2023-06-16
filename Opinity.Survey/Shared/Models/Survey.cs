using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace Opinity.Survey.Models
{
    public partial class OqtaneSurvey : IAuditable
    {
        public OqtaneSurvey()
        {
            OqtaneSurveyItem = new HashSet<OqtaneSurveyItem>();
        }

        [Key]
        public int SurveyId { get; set; }
        public int ModuleId { get; set; }
        public string SurveyName { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int? UserId { get; set; }
        public string AnonymousCookie { get; set; }

        public List<OqtaneSurveyItem> SurveyItem { get; set; }

        public virtual ICollection<OqtaneSurveyItem> OqtaneSurveyItem { get; set; }
    }

    public partial class OqtaneSurveyItem
    {
        public OqtaneSurveyItem()
        {
            OqtaneSurveyAnswer = new HashSet<OqtaneSurveyAnswer>();
            OqtaneSurveyItemOption = new HashSet<OqtaneSurveyItemOption>();
        }


        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int Survey { get; set; }
        public string ItemLabel { get; set; }
        public string ItemType { get; set; }
        public string ItemValue { get; set; }
        public int Position { get; set; }
        public int Required { get; set; }
        public int? SurveyChoiceId { get; set; }
        public string AnswerValueString { get; set; }
        public IEnumerable<string> AnswerValueList { get; set; }
        public DateTime? AnswerValueDateTime { get; set; }
        public List<OqtaneSurveyItemOption> SurveyItemOption { get; set; }
        public List<OqtaneAnswerResponse> AnswerResponses { get; set; }

        public virtual OqtaneSurvey SurveyNavigation { get; set; }
        public virtual ICollection<OqtaneSurveyAnswer> OqtaneSurveyAnswer { get; set; }
        public virtual ICollection<OqtaneSurveyItemOption> OqtaneSurveyItemOption { get; set; }
    }

    public class OqtaneSurveyItemOption
    {
        public int Id { get; set; }
        public int SurveyItem { get; set; }
        public string OptionLabel { get; set; }

        public virtual OqtaneSurveyItem SurveyItemNavigation { get; set; }
    }
    public class OqtaneAnswerResponse
{
        public string OptionLabel { get; set; }
        public double Responses { get; set; }
}

    public partial class OqtaneSurveyAnswer
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int SurveyItemId { get; set; }
        public string AnswerValue { get; set; }
        public DateTime AnswerValueDateTime { get; set; }
        public int? UserId { get; set; }
        public string AnonymousCookie { get; set; }

        public virtual OqtaneSurveyItem SurveyItem { get; set; }
    }


}