using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetDetailMemberChapter : IEntityProcView
    {
        public const string ProcName = "Proc_GetDetailMemberChapter";

        [Key]
        public int? ChapterId { get; set; }
        public string ChapterName { get; set; }
        public string RegionName { get; set; }
        public string ProvinceName { get; set; }
        public string RoleName { get; set; }
        public DateTime? DateJoin { get; set; }


        public Proc_GetDetailMemberChapter()
		{
		}

        public static IEntityProc GetEntityProc(int customerId)
        {
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            return new EntityProc(
                $"{ProcName} @CustomerId",
                new SqlParameter[] {
                    CustomerId
                }
            );
        }
    }
}
