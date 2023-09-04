using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetMembershipDuesReportAllNewMember : IEntityProcView
    {
        public const string ProcName = "Proc_GetMembershipDuesReportAllNewMember";
        [Key]
        public int Id { get; set; }
        public DateTime DateJoin { get; set; }
        public string FullName { get; set; }
        public string ProfessionName { get; set; }
        public string RoleName { get; set; }
        public DateTime EndDate { get; set; }


        public Proc_GetMembershipDuesReportAllNewMember()
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
