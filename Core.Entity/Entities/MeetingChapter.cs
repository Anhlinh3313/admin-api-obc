using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class MeetingChapter : EntityBasic
    {
        public MeetingChapter()
        {
        }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public string QrCodePath { get; set; }
        public int Loop { get; set; }
        public string Form { get; set; }
        public string Link { get; set; }
        public string Address { get; set; }
        public DateTime? DateEnd { get; set; }
        public int ChapterId { get; set; }
    }
}
