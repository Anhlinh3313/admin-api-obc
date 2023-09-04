using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListRegion : IEntityProcView
    {
        public const string ProcName = "Proc_GetListRegion";

        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string Code { get; set; }
        public string RegionName { get; set; }
        public string ProvinceName { get; set; }
        public bool IsActive { get; set; }
        public int Total { get; set; }


        public Proc_GetListRegion()
		{
		}

        public static IEntityProc GetEntityProc(string province, string keySearch, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            SqlParameter Province = new SqlParameter("@Province", province);
            if (string.IsNullOrEmpty(province)) Province.Value = "";
            return new EntityProc(
                $"{ProcName} @Province,@KeySearch,@PageNum, @PageSize",
                new SqlParameter[] {
                    Province,
                    KeySearch,
                    PageNum,
                    PageSize
                }
            );
        }
    }
}
