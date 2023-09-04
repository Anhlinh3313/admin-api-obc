using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetOpportunityReceiver : IEntityProcView
    {
        public const string ProcName = "Proc_GetOpportunityReceiver";
        [Key]
        public int GiverId { get; set; }
        public string GiverFullName { get; set; }
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
        public int OpportunityId { get; set; }
        public string StatusCode { get; set; }


        public Proc_GetOpportunityReceiver()
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
