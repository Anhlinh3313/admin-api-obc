using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Business.ViewModels.Permissions
{
    public class RolePermissionViewModel
    {
        public int RoleId { get; set; }
        public IEnumerable<PagePermissionViewModel> PagePermissions { get; set; } = new Collection<PagePermissionViewModel>();
    }
}
