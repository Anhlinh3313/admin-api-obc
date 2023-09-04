using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetClassifications : IEntityProcView
    {
        public const string ProcName = "Proc_GetClassifications";
        [Key]
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int CountChapter { get; set; }


        public Proc_GetClassifications()
		{
		}

        public static IEntityProc GetEntityProc()
        {
            return new EntityProc(
                $"{ProcName}",
                new SqlParameter[] {
                }
            );
        }
    }
}
