using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Business.ViewModels
{
    public class EnumData
    {
        public enum CustomerStatusEnum
        {
            [Description("Chưa đăng ký chapter")]
            FreeMember = 1,
            [Description("Chờ duyệt vào chapter")]
            PendingChapter = 2,
            [Description("Đã duyệt vào Chapter")]
            AcceptedChapter = 3,
            [Description("Chờ kích hoạt")]
            PendingActive = 4,
            [Description("Đã kích hoạt")]
            Active = 5
        }

        public enum CustomerRoleEnum
        {
            [Description("Thành viên")]
            PremiumMember = 1,
            [Description("Khách vãng lai")]
            FreeMember =2
        }

        public enum TransactionStatusEnum
        {
            [Description("Chờ kích hoạt")]
            PendingActive = 1,
            [Description("Đã kích hoạt")]
            Accepted = 2,
            [Description("Từ chối")]
            Cancel = 3,
        }

        public enum LogActionEnum
        {
            [Display(Name = "Tạo mới")]
            [Description("Tạo mới người dùng")]
            Create = 1,
            [Display(Name = "Chỉnh sửa")]
            [Description("Chỉnh sửa người dùng")]
            Update = 2,
        }

        public enum StatusOpportunity
        {
            [Description("Chưa liên hệ")]
            NotContact = 1,
            [Description("Đã liên hệ")]
            Contacted = 2,
            [Description("Không có phản hồi")]
            NoResponse = 3,
            [Description("Đã chốt giao dịch thành công")]
            Successful = 4,
            [Description("Không chốt giao dịch")]
            Cancel = 5,
            [Description("Cơ hội kinh doanh không phù hợp")]
            NotOpportunity = 6,
            [Description("Tuyệt mật")]
            TopSecret = 7,
        }

        public enum StatusFaceToFaceAndGuests
        {
            [Description("Chờ xác nhận")]
            Pending = 1,
            [Description("Đồng ý")]
            Accept = 2,
            [Description("Từ chối")]
            Cancel = 3
        }

        public enum StatusCustomerEvent
        {
            [Description("Tất cả")]
            All = 0,
            [Description("Đã đăng ký")]
            Accept = 1,
            [Description("Đã thanh toán")]
            Cancel = 2
        }

        public enum DayOfWeek
        {
            [Description("Chủ nhật")]
            Sunday = 0,
            [Description("Thứ 2")]
            Monday = 1,
            [Description("Thứ 3")]
            Tuesday = 2,
            [Description("Thứ 4")]
            Wednesday = 3,
            [Description("Thứ 5")]
            Thursday = 4,
            [Description("Thứ 6")]
            Friday = 5,
            [Description("Thứ 7")]
            Saturday = 6
        }

        public enum DayOfWeekEnglish
        {
            [Description("Sunday")]
            Sunday = 0,
            [Description("Monday")]
            Monday = 1,
            [Description("Tuesday")]
            Tuesday = 2,
            [Description("Wednesday")]
            Wednesday = 3,
            [Description("Thursday")]
            Thursday = 4,
            [Description("Friday")]
            Friday = 5,
            [Description("Saturday")]
            Saturday = 6
        }

        public enum NotifyType
        {
            Customer = 1,
            Opportunity = 2,
            Thanks = 3,
            FaceToFace = 4,
            Event = 5,
            Course = 6,
            Video = 7,
            FaceToFaceSuccess = 8,
            ChangeProfession = 9,
            ChangeFieldOperations = 10
        }
        public enum TypeNotify
        {
            [Description("Vào chapter")]
            AcceptChapter = 1,
            [Description("Huỷ vào chapter")]
            CancelChapter = 2,
            [Description("Thành viên")]
            AcceptPremium = 3,
            [Description("Nhận lời cảm ơn")]
            ThanksFor = 4,
            [Description("Nhận cơ hội")]
            OpportunityFor = 5,
            [Description("Nhận cuộc hẹn")]
            FaceToFaceFor = 6,
            [Description("Cuộc hẹn đồng ý")]
            FaceToFaceSuccess = 7,
            [Description("Cuộc hẹn từ chối")]
            FaceToFaceCancel = 8,
            [Description("Event thành công")]
            RegisterEvent = 9,
            [Description("Khoá học thành công")]
            RegisterCourse = 10,
            [Description("Video thành công")]
            RegisterVideo = 11,
            [Description("Event không thành công")]
            CancelRegisterEvent = 12,
            [Description("Khoá học không thành công")]
            CancelRegisterCourse = 13,
            [Description("Video không thành công")]
            CancelRegisterVideo = 14,
            [Description("Thay đổi ngành nghề thành công")]
            ChangeProfession = 15,
            [Description("Thay đổi lĩnh vực thành công")]
            ChangeFieldOperations = 16,
            [Description("Thay đổi ngành nghề không thành công")]
            CancelChangeProfession = 17,
            [Description("Thay đổi lĩnh vực không thành công")]
            CancelChangeFieldOperations = 18

        }
        public enum LoopMeetingChapter
        {
            [Description("Không lặp lại")]
            NoReplace = 1,
            [Description("Hàng ngày")]
            EveryDay = 2,
            [Description("Hàng tuần")]
            EveryWeek = 3,
            [Description("2 tuần")]
            TwoWeek = 4,
            [Description("Hàng tháng")]
            EveryMonth = 5
        }

        public enum LoopMeetingChapterEnglish
        {
            [Description("Do not repeat")]
            NoReplace = 1,
            [Description("Daily")]
            EveryDay = 2,
            [Description("Weekly")]
            EveryWeek = 3,
            [Description("2 week")]
            TwoWeek = 4,
            [Description("Monthly")]
            EveryMonth = 5
        }

    }
}
