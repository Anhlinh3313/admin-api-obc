using Core.Entity.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Core.Entity.Procedures
{
    public class Proc_PermissionDetail : IEntityProcView
    {
        public const string ProcName = "Proc_PermissionDetail";

        [Key]
        public bool IsAccess { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }

        public Proc_PermissionDetail()
        {
        }

        public static IEntityProc GetEntityProc(int userId, string aliasPath, int moduleId)
        {
            return new EntityProc(
                $"{ProcName} @UserId, @PageAlias, @ModuleId",
                new SqlParameter[] {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@PageAlias", aliasPath ?? aliasPath),
                    new SqlParameter("@ModuleId", moduleId)
                }
            );
        }
    }
}
