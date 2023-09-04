using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetCustomerProfile : IEntityProcView
    {
        public const string ProcName = "Proc_GetCustomerProfile";
        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Position { get; set; }
        public string BusinessName { get; set; }
        public string ProfessionName { get; set; }
        public string FieldOperationName { get; set; }
        public string ProfessionCode { get; set; }
        public string FieldOperationCode { get; set; }
        public string ChapterName { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string QrCodePath { get; set; }
        public string Address { get; set; }
        public string AvatarPath { get; set; }

        public Proc_GetCustomerProfile()
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
