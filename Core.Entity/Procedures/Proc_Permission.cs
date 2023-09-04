using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_Permission : IEntityProcView
    {
        public const string ProcName = "Core_Proc_Permission";

		[Key]
		public bool Result { get; set; }

		public Proc_Permission()
		{
		}

        public static IEntityProc GetEntityProc(int userId, string aliasPath)
        {
            return new EntityProc(
                $"{ProcName} @UserId, @PageAlias",
				new SqlParameter[] {
				    new SqlParameter("@UserId", userId),
                    new SqlParameter("@PageAlias", aliasPath ?? aliasPath)
                }
            );
        }
    }
}
