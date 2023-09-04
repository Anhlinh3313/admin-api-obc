using System;
using Core.Entity.Abstract;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Core.Entity.Procedures
{
    public class Proc_GetListCourseMobile : IEntityProcView
    {
        public const string ProcName = "Proc_GetListCourseMobile";
        [Key]
        public int CourseId { get; set; }
        public int CourseType { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int RowNum { get; set; }
        public int Scores { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string QrInformation { get; set; }
        public string Objects { get; set; }
        public string ImagePath { get; set; }
        public double Assess { get; set; }
        public bool Assessed { get; set; }
        public bool IsFee { get; set; }
        public bool Liked { get; set; }
        public int? SumLike { get; set; }
        public bool Shared { get; set; }
        public int? SumShare { get; set; }
        public int? SumComment { get; set; }
        public string StatusCertificate { get; set; }
        public string CertificatePath { get; set; }
        public DateTime? DateCertificate { get; set; }
        public int Total { get; set; }


        public Proc_GetListCourseMobile()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, int customerId, int courseType, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter CourseType = new SqlParameter("@CourseType", courseType);

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            return new EntityProc(
                $"{ProcName} @KeySearch,@CustomerId,@CourseType, @PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch,CustomerId, CourseType, PageNum, PageSize
                }
            );
        }
    }
}
