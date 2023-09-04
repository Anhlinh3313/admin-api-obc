using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListSearchHomeMobile : IEntityProcView
    {
        public const string ProcName = "Proc_GetListSearchHomeMobile";
        [Key]
        public int RowNum { get; set; }
        public int? CustomerId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string BusinessName { get; set; }
        public string FieldOperationsName { get; set; }
        public string AvatarPath { get; set; }
        public int StatusId { get; set; }
        public int? ChapterId { get; set; }
        public string ChapterName { get; set; }
        public string RegionName { get; set; }
        public string ProvinceName { get; set; }
        public int Total { get; set; }


        public Proc_GetListSearchHomeMobile()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, int typeId, int customerId, int pageNum, int pageSize)
        {
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter TypeId = new SqlParameter("@TypeId", typeId);
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            return new EntityProc(
                $"{ProcName} @KeySearch,@TypeId, @CustomerId,@PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch,TypeId,CustomerId,PageNum,PageSize
                }
            );
        }
    }
}
