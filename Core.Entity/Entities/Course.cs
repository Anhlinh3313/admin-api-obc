using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class Course : EntitySimple
    {
        public Course()
        {
        }
        public string Objects { get; set; }
        public int? NumberOfAttendees { get; set; }
        public bool Fee { get; set; }
        public bool IsActive { get; set; }
        public string Form { get; set; }
        public int? Scores { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string ImagePath { get; set; }
        public string VideoPath { get; set; }
        public string QrInformation { get; set; }
    }
}
