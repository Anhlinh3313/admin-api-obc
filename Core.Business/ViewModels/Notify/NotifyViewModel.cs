using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.Notify
{
    public class NotifyViewModel
    {
        public NotifyViewModel()
        {
            
        }
        public int RowNum { get; set; }
        public int NotifyId { get; set; }
        public int NotifyTypeId { get; set; }
        public string NotifyTypeName { get; set; }
        public bool IsSeen { get; set; }
        public string Content { get; set; }
        public PopUp PopUp { get; set; }
        public string AvatarPath { get; set; }
        public int? OpportunityId { get; set; }
        public int? ThanksId { get; set; }
        public int? FaceToFaceId { get; set; }
        public int? EventId { get; set; }
        public int? CourseId { get; set; }
        public int? VideoId { get; set; }
        public string EventCode { get; set; }
        public string CourseCode { get; set; }
        public string VideoCode { get; set; }
        public int? ProfessionId { get; set; }
        public int? FieldOperationsId { get; set; }
        public DateTime CreatedWhen { get; set; }
        public string Keyword { get; set; }
        public int Total { get; set; }
    }

    public class PopUp
    {
        public PopUp()
        {
            
        }
        public string CustomerNameCancel { get; set; }
        public DateTime? DateCancel { get; set; }
        public string ReasonCancel { get; set; }
    }
}
