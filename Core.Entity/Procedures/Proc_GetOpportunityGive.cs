using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetOpportunityGive : IEntityProcView
    {
        public const string ProcName = "Proc_GetOpportunityGive";
        [Key]
        public int ReceiverId { get; set; }
        public string ReceiverFullName { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string StatusName { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public int ActionTypeId { get; set; }
        public string StatusCode { get; set; }


        public Proc_GetOpportunityGive()
		{
		}

        public static IEntityProc GetEntityProc(int opportunityId)
        {
            SqlParameter Id = new SqlParameter("@Id", opportunityId);

            return new EntityProc(
                $"{ProcName} @Id",
                new SqlParameter[] {
                    Id
                }
            );
        }
    }
}
