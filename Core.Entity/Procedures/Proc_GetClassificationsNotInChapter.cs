using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetClassificationsNotInChapter : IEntityProcView
    {
        public const string ProcName = "Proc_GetClassificationsNotInChapter";
        [Key]
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public int CountChapter { get; set; }


        public Proc_GetClassificationsNotInChapter()
		{
		}

        public static IEntityProc GetEntityProc(int chapterId)
        {
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);
            return new EntityProc(
                $"{ProcName} @ChapterId",
                new SqlParameter[] {
                    ChapterId
                }
            );
        }
    }
}
