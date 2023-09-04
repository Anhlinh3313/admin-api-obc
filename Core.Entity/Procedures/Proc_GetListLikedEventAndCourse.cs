using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListLikedEventAndCourse : IEntityProcView
    {
        public const string ProcName = "Proc_GetListLikedEventAndCourse";

        [Key]
        public int RowNum { get; set; }
        public string FormType { get; set; }
        public int? EventId { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }
        public int? EventType { get; set; }
        public string ShortDescription { get; set; }
        public int? CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int? CourseType { get; set; }
        public double? Assess { get; set; }
        public bool? Assessed { get; set; }
        public int? VideoId { get; set; }
        public string VideoCode { get; set; }
        public string VideoName { get; set; }
        public int? VideoType { get; set; }
        public string ImagePath { get; set; }
        public int? Scores { get; set;  }
        public DateTime DateLiked { get; set; }
        public int Total { get; set; }


        public Proc_GetListLikedEventAndCourse()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, int customerId, int pageNum, int pageSize)
        {
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";
            return new EntityProc(
                $"{ProcName} @KeySearch,@CustomerId, @PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch,CustomerId, PageNum, PageSize
                }
            );
        }
    }
}
