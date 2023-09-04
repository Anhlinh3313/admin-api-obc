using System;
namespace Core.Business.ViewModels.Permissions
{
    public class PagePermissionViewModel
    {
		public int PageId { get; set; }
		public bool IsAccess { get; set; }
		public bool IsAdd { get; set; }
		public bool IsEdit { get; set; }
		public bool IsDelete { get; set; }
    }
}
