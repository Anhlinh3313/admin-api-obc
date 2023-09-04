using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetMembershipDuesReportAllMemberLate : IEntityProcView
    {
        public const string ProcName = "Proc_GetMembershipDuesReportAllMemberLate";
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ProfessionName { get; set; }
        public DateTime EndDate { get; set; }


        public Proc_GetMembershipDuesReportAllMemberLate()
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
