using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListCustomerPending : IEntityProcView
    {
        public const string ProcName = "Proc_GetListCustomerPending";

        [Key]
        public int CustomerId { get; set; }
        public int RowNum { get; set; }
        public int MemberType { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string FieldOperationsName { get; set; }
        public string ProfessionName { get; set; }
        public string AvatarPath { get; set; }
        public double? Assess { get; set; }
        public int? SumAssess { get; set; }
        public string NewProfession { get; set; }
        public string NewFieldOperations { get; set; }
        public int Total { get; set; }


        public Proc_GetListCustomerPending()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch ,int status, int customerId, int pageNum, int pageSize)
        {
            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            SqlParameter Status = new SqlParameter("@Status", status);
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            return new EntityProc(
                $"{ProcName} @KeySearch,@Status,@CustomerId, @PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch, Status, CustomerId, PageNum, PageSize
                }
            );
        }
    }
}
