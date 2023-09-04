using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetChapterInformation : IEntityProcView
    {
        public const string ProcName = "Proc_GetChapterInformation";

        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string ChapterName { get; set; }
        public string RegionName { get; set; }
        public string ProvinceName { get; set; }
        public int CountMember { get; set; }
        public bool IsActive { get; set; }


        public Proc_GetChapterInformation()
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
