using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class User : EntityBasic
    {
        public User()
        {
        }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public DateTime? VerificationDateTime { get; set; }
        public string VerificationCode { get; set; }
    }
}
