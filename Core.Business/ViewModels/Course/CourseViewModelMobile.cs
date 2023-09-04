using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class CourseViewModelMobile
    {
        public CourseViewModelMobile()
        {
            
        }
        public int CourseId { get; set; }
        public int CourseType { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int RowNum { get; set; }
        public TimeCourseMobile[] TimeCourses { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string QrInformation { get; set; }
        public string Objects { get; set; }
        public int Scores { get; set; }
        public double Assess { get; set; }
        public bool Assessed { get; set; }
        public string[] ImagePath { get; set; }
        public bool IsFee { get; set; }
        public bool Liked { get; set; }
        public int? SumLike { get; set; }
        public bool Shared { get; set; }
        public int? SumShare { get; set; }
        public int? SumComment { get; set; }
        public string StatusCertificate { get; set; }
        public string CertificatePath { get; set; }
        public DateTime? DateCertificate { get; set; }
        public int Total { get; set; }
    }

    public class TimeCourseMobile
    {
        public TimeCourseMobile()
        {
            
        }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}
