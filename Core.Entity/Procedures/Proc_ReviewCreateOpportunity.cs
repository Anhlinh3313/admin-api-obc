using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_ReviewCreateOpportunity : IEntityProcView
    {
        public const string ProcName = "Proc_ReviewCreateOpportunity";
        [Key]
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string StatusName { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }


        public Proc_ReviewCreateOpportunity()
		{
		}

        public static IEntityProc GetEntityProc(int opportunityId, int customerId)
        {
            SqlParameter Id = new SqlParameter("@Id", opportunityId);
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);

            return new EntityProc(
                $"{ProcName} @Id,@CustomerId",
                new SqlParameter[] {
                    Id,CustomerId
                }
            );
        }
    }
}
