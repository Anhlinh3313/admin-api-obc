using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListRecentEvent : IEntityProcView
    {
        public const string ProcName = "Proc_GetListRecentEvent";

        [Key]
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImagePath { get; set; }


        public Proc_GetListRecentEvent()
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
