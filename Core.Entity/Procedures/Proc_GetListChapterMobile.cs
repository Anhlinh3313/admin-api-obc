using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListChapterMobile : IEntityProcView
    {
        public const string ProcName = "Proc_GetListChapterMobile";

        [Key]
        public virtual int Id { get; set; }
        public virtual DateTime? CreatedWhen { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual DateTime? ModifiedWhen { get; set; }
        public virtual int? ModifiedBy { get; set; }
        public virtual string ConcurrencyStamp { get; set; }
        public virtual bool IsEnabled { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public int RegionId { get; set; }
        public int ProvinceId { get; set; }


        public Proc_GetListChapterMobile()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch)
        {
            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";
            return new EntityProc(
                $"{ProcName} @KeySearch",
                new SqlParameter[] {
                    KeySearch
                }
            );
        }
    }
}
