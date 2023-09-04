using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetMembershipDuesReportAllMember : IEntityProcView
    {
        public const string ProcName = "Proc_GetMembershipDuesReportAllMember";
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ProfessionName { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
        public DateTime EndDate { get; set; }


        public Proc_GetMembershipDuesReportAllMember()
		{
		}

        public static IEntityProc GetEntityProc(int chapterId)
        {
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);

            return new EntityProc(
                $"{ProcName} @ChapterId",
                new SqlParameter[] {
                    ChapterId
                }
            );
        }
    }
}
