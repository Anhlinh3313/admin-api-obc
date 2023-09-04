using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetTransactionCEUInKpiWeb : IEntityProcView
    {
        public const string ProcName = "Proc_GetTransactionCEUInKpiWeb";

        [Key]
        public int Id { get; set; }
        public string ActionName { get; set; }
        public int Value { get; set; }


        public Proc_GetTransactionCEUInKpiWeb()
		{
		}

        public static IEntityProc GetEntityProc(int customerId, DateTime fromDate)
        {
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);

            return new EntityProc(
                $"{ProcName} @CustomerId,@FromDate",
                new SqlParameter[] {
                    CustomerId,FromDate
                }
            );
        }
    }
}
