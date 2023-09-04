using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetChapterMemberCompany : IEntityProcView
    {
        public const string ProcName = "Proc_GetChapterMemberCompany";
        [Key]
        public long Id { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string ProfessionName { get; set; }
        public string Note { get; set; }


        public Proc_GetChapterMemberCompany()
		{
		}

        public static IEntityProc GetEntityProc(int chapterId, DateTime dateSearch)
        {
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);
            SqlParameter DateSearch = new SqlParameter("@DateSearch", dateSearch);
            return new EntityProc(
                $"{ProcName} @ChapterId, @DateSearch",
                new SqlParameter[] {
                    ChapterId, DateSearch
                }
            );
        }
    }
}
