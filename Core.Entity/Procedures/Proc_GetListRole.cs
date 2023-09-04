using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListRole : IEntityProcView
    {
        public const string ProcName = "Proc_GetListRole";
        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string RoleTypeName { get; set; }
        public int Total { get; set; }


        public Proc_GetListRole()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";
            return new EntityProc(
                $"{ProcName} @KeySearch, @PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch, PageNum, PageSize
                }
            );
        }
    }
}
