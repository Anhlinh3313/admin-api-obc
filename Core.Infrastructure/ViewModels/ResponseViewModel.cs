using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Infrastructure.ViewModels
{
    public class ResponseViewModel
    {
        public ResponseViewModel()
        {
        }

        public ResponseViewModel(bool isSuccess, string message, int? dataCount, dynamic data, Exception exception)
        {
            IsSuccess = isSuccess;
            Message = message;
            DataCount = dataCount;
            Data = data;
            Exception = exception;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int? DataCount { get; set; }
        public dynamic Data { get; set; }
        public Exception Exception { get; set; }

        public static ResponseViewModel CreateSuccess(object data = null, string message = null, int? dataCount = null)
        {
            return new ResponseViewModel(true, message, dataCount, data, null);
        }

        public static ResponseViewModel CreateError(string message = null)
        {
            return new ResponseViewModel(false, message, null, null, null);
        }

        public static ResponseViewModel CreateErrorObject(object data)
        {
            return new ResponseViewModel(false, null, null, data, null);
        }

        public static ResponseViewModel CreateErrorObject(Exception ex)
        {
            return new ResponseViewModel(false, null, null, null, ex);
        }
    }
}
