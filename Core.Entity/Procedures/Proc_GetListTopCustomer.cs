using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListTopCustomer : IEntityProcView
    {
        public const string ProcName = "Proc_GetListTopCustomer";

        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string FullName { get; set; }
        public string AvatarPath { get; set; }
        public double AvgAssess { get; set; }
        public string BusinessName { get; set; }
        public string ProfessionName { get; set; }
        public string FieldOperationsName { get; set; }
        public string ChapterName { get; set; }
        public int Total { get; set; }


        public Proc_GetListTopCustomer()
		{
		}

        public static IEntityProc GetEntityProc(int pageNum, int pageSize, string language)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter Language = new SqlParameter("@Language", language);
            if (string.IsNullOrEmpty(language)) Language.Value = "";
            return new EntityProc(
                $"{ProcName} @PageNum,@PageSize,@Language",
                new SqlParameter[] {
                    PageNum, PageSize,Language
                }
            );
        }
    }
}
