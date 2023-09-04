using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListRegionWithProvince : IEntityProcView
    {
        public const string ProcName = "Proc_GetListRegionWithProvince";

        [Key]
        public int Id { get; set; }
        public string RegionName { get; set; }


        public Proc_GetListRegionWithProvince()
		{
		}

        public static IEntityProc GetEntityProc(string province, string keySearch)
        {

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            SqlParameter Province = new SqlParameter("@Province", province);
            if (string.IsNullOrEmpty(province)) Province.Value = "";
            return new EntityProc(
                $"{ProcName} @Province,@KeySearch",
                new SqlParameter[] {
                    Province,
                    KeySearch
                }
            );
        }
    }
}
