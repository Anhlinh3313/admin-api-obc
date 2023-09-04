using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetDetailBusiness : IEntityProcView
    {
        public const string ProcName = "Proc_GetDetailBusiness";

        [Key]
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string TaxCode { get; set; }
        public int? ProfessionId { get; set; }
        public int? FieldOperationsId { get; set; }
        public int? BusinessProvinceId { get; set; }
        public int? BusinessDistrictId { get; set; }
        public int? BusinessWardId { get; set; }
        public string BusinessAddress { get; set; }
        public string Position { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityCard { get; set; }
        public DateTime? IdentityCardDate { get; set; }
        public int? IdentityCardPlaceId { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }
        public int CustomerRoleId { get; set; }
        public string Gender { get; set; }
        public DateTime CreateDate { get; set; }

        public Proc_GetDetailBusiness()
		{
		}

        public static IEntityProc GetEntityProc(int id)
        {

            SqlParameter CustomerId = new SqlParameter("@CustomerId", id);
            return new EntityProc(
                $"{ProcName} @CustomerId",
                new SqlParameter[] {
                    CustomerId
                }
            );
        }
    }
}
