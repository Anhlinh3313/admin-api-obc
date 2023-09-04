using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListNotify : IEntityProcView
    {
        public const string ProcName = "Proc_GetListNotify";

        [Key]
        public int RowNum { get; set; }
        public int NotifyId { get; set; }
        public int NotifyTypeId { get; set; }
        public string CustomerNameCancel { get; set; }
        public DateTime? DateCancel { get; set; }
        public bool IsSeen { get; set; }
        public string ReasonCancel { get; set; }
        public string ChapterName { get; set; }
        public string ReceiverName { get; set; }
        public string AvatarPath { get; set; }
        public string EventName { get; set; }
        public string CourseName { get; set; }
        public string VideoName { get; set; }
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
        public  string ProfessionName { get; set; }
        public string ProfessionCode { get; set; }
        public string FieldOperationsName { get; set; }
        public string FieldOperationsCode { get; set; }
        public DateTime CreatedWhen { get; set; }
        public int Total { get; set; }


        public Proc_GetListNotify()
		{
		}

        public static IEntityProc GetEntityProc(int customerId, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);

            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            return new EntityProc(
                $"{ProcName} @CustomerId,@PageNum, @PageSize",
                new SqlParameter[] {
                    CustomerId,
                    PageNum,
                    PageSize
                }
            );
        }
    }
}
