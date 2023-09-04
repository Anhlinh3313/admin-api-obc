using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListAssessCustomer : IEntityProcView
    {
        public const string ProcName = "Proc_GetListAssessCustomer";
        [Key]
        public int RowNum { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string FieldOperationsName { get; set; }
        public string ChapterName { get; set; }
        public int AssessValue { get; set; }
        public string AssessComment { get; set; }
        public string AvatarPath { get; set; }
        public int Total { get; set; }


        public Proc_GetListAssessCustomer()
		{
		}

        public static IEntityProc GetEntityProc(int customerId, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);

            return new EntityProc(
                $"{ProcName} @CustomerId, @PageNum, @PageSize",
                new SqlParameter[] {
                    CustomerId, PageNum, PageSize
                }
            );
        }
    }
}
