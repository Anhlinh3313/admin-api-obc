using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetMembershipDuesReportAllMemberExpired : IEntityProcView
    {
        public const string ProcName = "Proc_GetMembershipDuesReportAllMemberExpired";
        [Key]
        public long RowNum { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ProfessionName { get; set; }
        public DateTime DateOut { get; set; }
        public string Status { get; set; }


        public Proc_GetMembershipDuesReportAllMemberExpired()
		{
		}

        public static IEntityProc GetEntityProc(int chapterId, DateTime fromDate)
        {
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);
            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);

            return new EntityProc(
                $"{ProcName} @ChapterId,@FromDate",
                new SqlParameter[] {
                    ChapterId,FromDate
                }
            );
        }
    }
}
