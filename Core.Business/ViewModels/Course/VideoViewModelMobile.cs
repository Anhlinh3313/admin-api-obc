using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;
using Microsoft.AspNetCore.Http;

namespace Core.Business.ViewModels.Course
{
    public class VideoViewModelMobile
    {
        public VideoViewModelMobile()
        {
            
        }
        public int VideoId { get; set; }
        public int VideoType { get; set; }
        public string VideoName { get; set; }
        public string VideoCode { get; set; }
        public int RowNum { get; set; }
        public TimeCourseMobile[] TimeVideos { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string QrInformation { get; set; }
        public string Objects { get; set; }
        public string[] ImagePath { get; set; }
        public bool IsFee { get; set; }
        public bool Liked { get; set; }
        public int? SumLike { get; set; }
        public bool Shared { get; set; }
        public int? SumShare { get; set; }
        public int Scores { get; set; }
        public string[] VideoPath { get; set; }
        public double Assess { get; set; }
        public bool Assessed { get; set; }
        public int? SumComment { get; set; }
        public int Total { get; set; }
    }
}
