using System;
namespace Core.Business.Services.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
        }

		public string Name { get; set; }
		public string Version { get; set; }
		public string Salt { get; set; }
    }
}
