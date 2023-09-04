using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelUploadCertificate
    {
        public CourseViewModelUploadCertificate()
        {
            
        }
        public int CourseId { get; set; }
        public IFormFile File { get; set; }
    }
}
