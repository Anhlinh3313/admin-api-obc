using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.MeetingChapter
{
    public class MeetingChapterViewModel : IEntityBase
    {
        public MeetingChapterViewModel()
        {
            
        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string MeetingName { get; set; }
        public DateTime Time { get; set; }
        public string QrCodePath { get; set; }
        public int Loop { get; set; }
        public string Form { get; set; }
        public string Link { get; set; }
        public string Address { get; set; }
        public DateTime? DateEnd { get; set; }
    }

    public class ListMeetingChapterViewModel
    {
        public ListMeetingChapterViewModel()
        {

        }
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string MeetingName { get; set; }
        public string Time { get; set; }
        public string QrCodePath { get; set; }
        public string Loop { get; set; }
        public string Form { get; set; }
        public string Link { get; set; }
        public string Address { get; set; }
        public DateTime? DateEnd { get; set; }
        public int SumGuests { get; set; }
    }

    public class LoopMeetingChapterViewModel
    {
        public LoopMeetingChapterViewModel()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MeetingChapterViewModelInGuests
    {
        public MeetingChapterViewModelInGuests()
        {

        }
        public string ChapterName { get; set; }
        public List<TimeModel> Time { get; set; }
        public string Link { get; set; }
        public string Address { get; set; }
    }
    public class TimeModel
    {
        public TimeModel()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int MeetingChapterId { get; set; }
    }
}
