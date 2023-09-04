using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Guests : EntityBasic
    {
        public Guests()
        {
        }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public DateTime? MeetingDate { get; set; }
        public string MeetingWhere { get; set; }
        public int? EventId { get; set; }
        public int? StatusId { get; set; }
        public int MeetingChapterId { get; set; }
        public bool? IsCheckin { get; set; }
        public DateTime? DateCheckIn { get; set; }
        public bool? IsGuests { get; set; }
    }
}
