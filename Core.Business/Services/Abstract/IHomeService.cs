using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Business.ViewModels.Chapter;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IHomeService
    {
        JsonResult GetTypeSearchHomeMobile(int currentUserId);
        JsonResult GetListSearchHomeMobile(string keySearch, string typeId, int customerId, int pageNum, int pageSize);
    }
}
