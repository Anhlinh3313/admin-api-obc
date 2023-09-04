using System;
namespace Core.Business.ViewModels
{
    public class UploadFileViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileBase64String { get; set; }

        public UploadFileViewModel()
        {
        }
    }
}
