using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entity.Abstract;

namespace Core.Business.ViewModels.RolePage
{
    public class RolePageViewModelView
    {
        public RolePageViewModelView()
        {
            
        }
        public string PageName { get; set; }
        public RolePageViewModelViewItem[] RolePage { get; set; }
    }

    public class RolePageViewModelViewItem
    {
        public int RolePageId { get; set; }
        public string PageName { get; set; }
        public bool IsView { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public int PageId { get; set; }
        public int RoleId { get; set; }
    }
}
