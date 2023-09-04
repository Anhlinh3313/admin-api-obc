using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Core.Infrastructure.Enum;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Collections.Generic;
using Core.Infrastructure.ViewModels;

namespace Core.Infrastructure.Utils
{
    public static class JsonUtil
    {
        public static JsonResult Create(ResponseViewModel viewModel)
        {
            if (viewModel.IsSuccess)
                return Success(viewModel);
            else
                return Error(viewModel);
        }

        public static JsonResult Success(object data = null, string message = null, int? dataCount = null)
        {
            dynamic obj;

            if (dataCount.HasValue)
                obj = new { isSuccess = StatusResponse.Success, message = message, dataCount = dataCount, data = data };
            else
                obj = new { isSuccess = StatusResponse.Success, message = message, data = data };

            return new JsonResult(obj);
        }

        public static JsonResult Success(ResponseViewModel viewModel)
        {
            dynamic obj;

            if (viewModel.DataCount.HasValue)
                obj = new { isSuccess = StatusResponse.Success, message = viewModel.Message, dataCount = viewModel.DataCount, data = viewModel.Data };
            else
                obj = new { isSuccess = StatusResponse.Success, message = viewModel.Message, data = viewModel.Data };

            return new JsonResult(obj);
        }

        public static JsonResult Error(string message)
        {
            return Error(message, null);
        }

        public static JsonResult Error(List<KeyValuePair<string, object>> data)
        {
            return Error(null, data);
        }

        public static JsonResult Error(string message, object data)
        {
            return new JsonResult(new { isSuccess = StatusResponse.Error, message = message, data = data });
        }

        public static JsonResult Error(ResponseViewModel viewModel)
        {
            return new JsonResult(new { isSuccess = StatusResponse.Error, message = viewModel.Message, data = viewModel.Data });
        }

        public static JsonResult Error(ModelStateDictionary modelState)
        {
            var errorList = modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => (e.Exception == null) ? e.ErrorMessage : e.Exception.Message).FirstOrDefault()
            );

            return new JsonResult(new { isSuccess = StatusResponse.Error, message = (string)null, data = errorList });
        }
    }
}
