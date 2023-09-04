using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_CheckUniqueFieldOperationsChapter : IEntityProcView
    {
        public const string ProcName = "Proc_CheckUniqueFieldOperationsChapter";

		[Key]
		public int? CustomerId { get; set; }
        public int? BusinessId { get; set; }


        public Proc_CheckUniqueFieldOperationsChapter()
		{
		}

        public static IEntityProc GetEntityProc(int participatingChapterId, int fieldOperationsId)
        {
            SqlParameter ParticipatingChapterId = new SqlParameter("@ParticipatingChapterId", participatingChapterId);
            SqlParameter FieldOperationsId = new SqlParameter("@FieldOperationsId", fieldOperationsId);
            return new EntityProc(
                $"{ProcName} @ParticipatingChapterId,@FieldOperationsId",
				new SqlParameter[] {
                    ParticipatingChapterId,FieldOperationsId
                }
            );
        }
    }
}
