using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetMeetingChapterWithCustomerId : IEntityProcView
    {
        public const string ProcName = "Proc_GetMeetingChapterWithCustomerId";

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedWhen { get; set; }


        public Proc_GetMeetingChapterWithCustomerId()
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
