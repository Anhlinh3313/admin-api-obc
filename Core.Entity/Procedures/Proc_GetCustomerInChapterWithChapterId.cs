using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetCustomerInChapterWithChapterId : IEntityProcView
    {
        public const string ProcName = "Proc_GetCustomerInChapterWithChapterId";
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string Position { get; set; }
        public string FieldOperationsName { get; set; }
        public string AvatarPath { get; set; }
        public int RGI { get; set; }
        public int RGO { get; set; }
        public int RRI { get; set; }
        public int RRO { get; set; }
        public int V { get; set; }
        public int F2F { get; set; }
        public long TYFCB { get; set; }
        public int CEU { get; set; }


        public Proc_GetCustomerInChapterWithChapterId()
		{
		}

        public static IEntityProc GetEntityProc(int chapterId, DateTime fromDate)
        {
            SqlParameter ChapterId = new SqlParameter("@ChapterId", chapterId);
            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);

            return new EntityProc(
                $"{ProcName} @ChapterId,@FromDate",
                new SqlParameter[] {
                    ChapterId,FromDate
                }
            );
        }
    }
}
