using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListChapter : IEntityProcView
    {
        public const string ProcName = "Proc_GetListChapter";

        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string Code { get; set; }
        public string ChapterName { get; set; }
        public string RegionName { get; set; }
        public string ProvinceName { get; set; }
        public int? CountMember { get; set; }
        public bool IsActive { get; set; }
        public string QrCodePath { get; set; }
        public int Total { get; set; }


        public Proc_GetListChapter()
		{
		}

        public static IEntityProc GetEntityProc(string province, string region, string keySearch, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            SqlParameter Province = new SqlParameter("@Province", province);
            if (string.IsNullOrEmpty(province)) Province.Value = "";

            SqlParameter Region = new SqlParameter("@Region", region);
            if (string.IsNullOrEmpty(region)) Region.Value = "";
            return new EntityProc(
                $"{ProcName} @Province,@Region,@KeySearch,@PageNum, @PageSize",
                new SqlParameter[] {
                    Province,
                    Region,
                    KeySearch,
                    PageNum,
                    PageSize
                }
            );
        }
    }
}
