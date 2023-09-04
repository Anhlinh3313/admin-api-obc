using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.AbsenceMedical
{
    public class AbsenceMedicalViewModel
    {
        public AbsenceMedicalViewModel()
        {
            
        }
        public string Content { get; set; }
        public IFormFile File { get; set; }
        public int MeetingChapterId { get; set; }
    }
}
