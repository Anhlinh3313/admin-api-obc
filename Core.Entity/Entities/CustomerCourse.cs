using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class CustomerCourse : EntityBasic
    {
        public CustomerCourse()
        {
        }
        public int CustomerId { get; set; }
        public int CourseId { get; set; }
        public bool Status { get; set; }
        public string ImagePath { get; set; }
        public string Note { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int? TimeVideo { get; set; }
        public bool? StatusCertificate { get; set; }
        public DateTime? DateCertificate { get; set; }
    }
}
