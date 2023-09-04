using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListBusiness : IEntityProcView
    {
        public const string ProcName = "Proc_GetListBusiness";

        [Key]
        public int Id { get; set; }
        public int RowNum { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ProvinceName { get; set; }
        public string CustomerRoleName { get; set; }
        public string BusinessName { get; set; }
        public string FieldOperationName { get; set; }
        public string ParticipatingProvinceName { get; set; }
        public string ProfessionName { get; set; }
        public string RegionName { get; set; }
        public string ChapterName { get; set; }
        public DateTime? DateJoin { get; set; }
        public DateTime CreateWhen { get; set; }
        public string QrCodePath { get; set; }
        public bool IsActive { get; set; }
        public int Total { get; set; }


        public Proc_GetListBusiness()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, string province, string profession, string fieldOperation, int customerRole, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            SqlParameter Province = new SqlParameter("@Province", province);
            if (string.IsNullOrEmpty(province)) Province.Value = "";

            SqlParameter FieldOperation = new SqlParameter("@FieldOperation", fieldOperation);
            if (string.IsNullOrEmpty(fieldOperation)) FieldOperation.Value = "";


            SqlParameter Profession = new SqlParameter("@Profession", profession);
            if (string.IsNullOrEmpty(profession)) Profession.Value = "";

            SqlParameter CustomerRole = new SqlParameter("@CustomerRole", customerRole);
            return new EntityProc(
                $"{ProcName} @KeySearch, @province,@Profession, @FieldOperation, @CustomerRole, @PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch, Province,Profession, FieldOperation, CustomerRole, PageNum, PageSize
                }
            );
        }
    }
}
