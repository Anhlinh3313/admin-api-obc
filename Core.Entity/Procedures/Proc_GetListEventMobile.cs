using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListEventMobile : IEntityProcView
    {
        public const string ProcName = "Proc_GetListEventMobile";

        [Key]
        public int EventId { get; set; }
        public int EventType { get; set; }
        public string EventName { get; set; }
        public string EventCode { get; set; }
        public int RowNum { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string LinkInformationQrCodePath { get; set; }
        public string LinkCheckInQrCodePath { get; set; }
        public string Objects { get; set; }
        public bool IsFee { get; set; }
        public string ImagePath { get; set; }
        public bool Liked { get; set; }
        public int? SumLike { get; set; }
        public bool Shared { get; set; }
        public int? SumShare { get; set; }
        public int Total { get; set; }


        public Proc_GetListEventMobile()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, int customerId, int eventType, int pageNum, int pageSize)
        {
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);

            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter EventType = new SqlParameter("@EventType", eventType);

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";
            return new EntityProc(
                $"{ProcName} @KeySearch,@CustomerId,@EventType,@PageNum, @PageSize",
                new SqlParameter[] {
                    KeySearch,
                    CustomerId,
                    EventType,
                    PageNum,
                    PageSize
                }
            );
        }
    }
}
