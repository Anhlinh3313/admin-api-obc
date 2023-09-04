using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetIndicators : IEntityProcView
    {
        public const string ProcName = "Proc_GetIndicators";

        [Key]
        public int Id { get; set; }
        public string ActionName { get; set; }
        public long? ValueMonth { get; set; }
        public long? ValueHistory { get; set; }


        public Proc_GetIndicators()
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
