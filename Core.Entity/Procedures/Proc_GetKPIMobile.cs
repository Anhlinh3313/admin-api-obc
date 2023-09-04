using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetKPIMobile : IEntityProcView
    {
        public const string ProcName = "Proc_GetKPIMobile";
        [Key]
        public int Id { get; set; }
        public string ActionName { get; set; }
        public long Value { get; set; }


        public Proc_GetKPIMobile()
		{
		}

        public static IEntityProc GetEntityProc(int customerId, int monthFrom, int yearFrom, int monthTo, int yearTo, string language)
        {
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter MonthFrom = new SqlParameter("@MonthFrom", monthFrom);
            SqlParameter YearFrom = new SqlParameter("@YearFrom", yearFrom);
            SqlParameter MonthTo = new SqlParameter("@MonthTo", monthTo);
            SqlParameter YearTo = new SqlParameter("@YearTo", yearTo);
            SqlParameter Language = new SqlParameter("@Language", language);


            return new EntityProc(
                $"{ProcName} @CustomerId, @MonthFrom, @YearFrom,@MonthTo,@YearTo,@Language",
                new SqlParameter[] {
                    CustomerId, MonthFrom, YearFrom, MonthTo, YearTo,Language
                }
            );
        }
    }
}
