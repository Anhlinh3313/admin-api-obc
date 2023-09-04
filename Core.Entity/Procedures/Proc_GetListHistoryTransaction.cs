using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using Core.Entity.Abstract;

namespace Core.Entity.Procedures
{
    public class Proc_GetListHistoryTransaction : IEntityProcView
    {
        public const string ProcName = "Proc_GetListHistoryTransaction";
        [Key]
        public int RowNum { get; set; }
        public int? Id { get; set; }
        public string ActionType { get; set; }
        public int ActionTypeId { get; set; }
        public string StatusName { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverBusiness { get; set; }
        public string GiverName { get; set; }
        public string GiverBusiness { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public int? Level { get; set; }
        public long? Value { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime? ExchangeTime { get; set; }
        public string ImagePathReceive { get; set; }
        public string ImagePathGive { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string MeetingWhere { get; set; }
        public DateTime? MeetingDate { get; set; }
        public string MeetingName { get; set; }
        public int? CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public int? Scores { get; set; }
        public string CourseStatus { get; set; }
        public int? VideoId { get; set; }
        public string VideoName { get; set; }
        public string VideoCode { get; set; }
        public string VideoStatus { get; set; }
        public DateTime CreatedWhen { get; set; }
        public int Total { get; set; }


        public Proc_GetListHistoryTransaction()
		{
		}

        public static IEntityProc GetEntityProc(string keySearch, int customerId, int transactionActionId, int month, int year, int pageNum, int pageSize)
        {
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            SqlParameter TransactionActionId = new SqlParameter("@TransactionActionId", transactionActionId);
            SqlParameter PageNum = new SqlParameter("@PageNum", pageNum);
            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            SqlParameter Month = new SqlParameter("@Month", month);
            SqlParameter Year = new SqlParameter("@Year", year);

            SqlParameter KeySearch = new SqlParameter("@KeySearch", keySearch);
            if (string.IsNullOrEmpty(keySearch)) KeySearch.Value = "";

            return new EntityProc(
                $"{ProcName} @KeySearch,@CustomerId,@TransactionActionId,@Month, @Year,@PageNum,@PageSize",
                new SqlParameter[] {
                    KeySearch,CustomerId,TransactionActionId,Month,Year,PageNum,PageSize
                }
            );
        }
    }
}
