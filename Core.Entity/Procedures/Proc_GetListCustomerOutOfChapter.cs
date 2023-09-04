using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListCustomerOutOfChapter : IEntityProcView
    {
        public const string ProcName = "Proc_GetListCustomerOutOfChapter";
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string Position { get; set; }
        public string FieldOperationsName { get; set; }
        public string AvatarPath { get; set; }
        public int Total { get; set; }


        public Proc_GetListCustomerOutOfChapter()
		{
		}

        public static IEntityProc GetEntityProc(int customerId, string fullName, string businessName, string fieldOperationsName, string provinceName, string keySearch, int type)
        {
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter Type = new SqlParameter("@Type", type);

            SqlParameter FullName = new SqlParameter("@FullName", fullName);
            if (string.IsNullOrEmpty(fullName)) FullName.Value = "";

            SqlParameter BusinessName = new SqlParameter("@BusinessName", businessName);
            if (string.IsNullOrEmpty(businessName)) BusinessName.Value = "";

            SqlParameter FieldOperationsName = new SqlParameter("@FieldOperationsName", fieldOperationsName);
            if (string.IsNullOrEmpty(fieldOperationsName)) FieldOperationsName.Value = "";

            SqlParameter ProvinceName = new SqlParameter("@ProvinceName", provinceName);
            if (string.IsNullOrEmpty(provinceName)) ProvinceName.Value = "";

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            return new EntityProc(
                $"{ProcName} @CustomerId, @FullName, @BusinessName, @FieldOperationsName, @ProvinceName, @KeySearch, @Type",
                new SqlParameter[] {
                    CustomerId, FullName, BusinessName, FieldOperationsName, ProvinceName, KeySearch, Type
                }
            );
        }
    }
}
