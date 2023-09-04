using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetAllMeetingChapterExpired : IEntityProcView
    {
        public const string ProcName = "Proc_GetAllMeetingChapterExpired";
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public int Loop { get; set; }
        public string Form { get; set; }
        public string Link { get; set; }
        public string Address { get; set; }
        public DateTime? DateEnd { get; set; }
        public int ChapterId { get; set; }

        public Proc_GetAllMeetingChapterExpired()
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
