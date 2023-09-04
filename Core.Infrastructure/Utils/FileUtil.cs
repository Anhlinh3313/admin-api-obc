using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Infrastructure.ViewModels;
using Microsoft.DotNet.PlatformAbstractions;

namespace Core.Infrastructure.Utils
{
    public static class FileUtil
    {
        public static string SaveFile(string targetFolder, FileViewModel fileViewModel)
        {
            DateTime currentDate = DateTime.Now;
            string fileName = fileViewModel.FileName;

            string dir = $@"{ApplicationEnvironment.ApplicationBasePath}{targetFolder}/{currentDate.ToString("yyyy-MM")}";
            string fullPath = $@"{dir}/{fileName}";

            //Check Directory
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            for (int i = 0; ; i++)
            {
                FileInfo fileInfo = new FileInfo(fullPath);

                if (fileInfo.Exists)
                {
                    string[] arr = fileName.Split('.');
                    //Xóa string copy (i)
                    arr[0] = arr[0].Replace($" ({i})", "");
                    //Tạo string copy (i + 1)
                    arr[0] = $"{arr[0]} ({i + 1})";
                    //
                    fileName = string.Join(".", arr);
                    //
                    fullPath = $@"{dir}/{fileName}";
                }
                else
                {
                    break;
                }
            }

            byte[] byteArray = Convert.FromBase64String(fileViewModel.FileBase64String);

            using (var fs = new FileStream($@"{dir}/{fileName}", FileMode.CreateNew, FileAccess.Write))
            {
                fs.Write(byteArray, 0, byteArray.Length);
            }
            return $@"{targetFolder}/{currentDate.ToString("yyyy-MM")}/{fileName}";
        }


        public static FileViewModel GetFile(string relativePath)
        {
            FileViewModel fileViewModel = new FileViewModel();
            string fullPath = $@"{ApplicationEnvironment.ApplicationBasePath}{relativePath}";
            //string fullPath = $@"D:\angular\BEEPTB\BEEPTB_SYSTEM_API\Core.Api\{relativePath}";

            FileInfo file = new FileInfo(fullPath);
            if (file.Exists)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
                fileViewModel.FileName = file.Name;
                fileViewModel.FileExtension = file.Extension;
                fileViewModel.FileBase64String = Convert.ToBase64String(bytes);
            }
            return fileViewModel;
        }

        public static bool IsBase64(string base64String)
        {
            if (base64String.Replace(" ", "").Length % 4 != 0)
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
