using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entity.Abstract;

namespace Core.Entity.Entities
{
    public class MeetingChapterCheckIn : EntityBasic
    {
        public MeetingChapterCheckIn()
        {
        }
        public int CustomerId { get; set; }
        public int MeetingChapterId { get; set; }
    }
}
