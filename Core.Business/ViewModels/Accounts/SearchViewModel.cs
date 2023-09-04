using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Business.ViewModels.Accounts
{
    public class SearchViewModel
    {
        public int? Id { get; set; }
        public string SearchText { get; set; }
        public string IdentityCard { get; set; }
        public string PhoneNumber { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoleId { get; set; }
        public int? UserTypeId { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string Cols { get; set; }
    }
}
