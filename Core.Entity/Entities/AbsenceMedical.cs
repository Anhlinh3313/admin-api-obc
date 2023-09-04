using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class AbsenceMedical : EntityBasic
    {
        public AbsenceMedical()
        {
        }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public int MeetingChapterId { get; set; }
    }
}
