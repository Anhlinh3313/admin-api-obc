using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Customer : EntityBasic
    {
        public Customer()
        {
        }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
        public int? ProvinceId { get; set; }
        public string IdentityCard { get; set; }
        public DateTime IdentityCardDate { get; set; }
        public int IdentityCardPlace { get; set; }
        public int? WardId { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int? CustomerRoleId { get; set; }
        public bool IsActive { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public int StatusId { get; set; }
        public string ReasonCancel { get; set; }
        public string Note { get; set; }
        public int? RoleId { get; set; }
        public DateTime? VerificationDateTime { get; set; }
        public string VerificationCode { get; set; }
        public string AvatarPath { get; set; }
        public string QrCodePath { get; set; }
        public string ExpoPushToken { get; set; }
        public string Language { get; set; }
        public string NewPhoneNumber { get; set; }
        public string NewEmail { get; set; }
    }
}
