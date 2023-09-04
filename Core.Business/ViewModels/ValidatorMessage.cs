using System;
namespace Core.Business.ViewModels
{
    public static class ValidatorMessage
    {
        public static class General
        {
			public static readonly string NotExist = "Dữ liệu không tồn tại";
			public static readonly string NotDestroy = "Dữ liệu đã được sử dụng, không thể xoá";
            public static readonly string NotDeActive = "Dữ liệu đã được sử dụng, không thể huỷ kích hoạt";
            public static readonly string ConcurrencyStamp = "Dữ liệu đã được người khác sử dụng";
			public static readonly string NameNotEmpty = "Tên không được để trống";
			public static readonly string CodeNotEmpty = "Mã không được để trống";
			public static readonly string UniqueName = "Tên đã tồn tại";
			public static readonly string UniqueCode = "Mã đã tồn tại";
			public static readonly string EmailInvalid = "Email không đúng định dạng";
        }

        public static class Account
        {
            public static readonly string NotExist = "Tài khoản không tồn tại";
            public static readonly string NotEmpty = "Email hoặc số điện thoại không được để trống";
            public static readonly string UserNameNotEmpty = "Tên đăng nhập không được để trống";
			public static readonly string FullNameNotEmpty = "Họ tên không được để trống";
            public static readonly string EmailInvalid = "Email không đúng định dạng";
            public static readonly string UniqueUserName = "Tên đăng nhập đã tồn tại";
            public static readonly string CodeNotEmpty = "Mã không được để trống";
            public static readonly string UniqueCode = "Mã đã tồn tại";
            public static readonly string PassWordNotEmpty = "Mật khẩu không được để trống";
            public static readonly string InvalidUserNamePassWord = "Tên đăng nhập hoặc mật khẩu không chính xác";
			public static readonly string AccountHasBeenBlock = "Tài khoản của bạn đã bị khoá";
			public static readonly string IdentityCardInvalid = "CMND không hợp lệ";
			public static readonly string CurrentPassWordNotEmpty = "Mật khẩu hiện tại không được để trống";
			public static readonly string CurrentPassWordInValid = "Mật khẩu hiện tại không chính xác";
            public static readonly string CurrentPassWordInValidEnglish = "Current password is incorrect";
            public static readonly string NewPassWordNotEmpty = "Mật khẩu mới không được để trống";
            public static readonly string NewPassWordNotDuplicated = "Mật khẩu mới không được để trùng mật khẩu hiện tại";
            public static readonly string NewPassWordNotDuplicatedEnglish = "The new password cannot be the same as the current password";
            public static readonly string UniqueEmail = "Email đã tồn tại";
            public static readonly string EmailNotEmpty = "Email không được để trống";
            public static readonly string UniquePhone = "Số điện thoại đã tồn tại";
            public static readonly string PhonenumberNotEmpty = "Số điện thoại không được để trống";
            public static readonly string AddressNotEmpty = "Địa chỉ không được để trống";
			public static readonly string RoleNotEmpty = "Vai trò không được để trống";
            public static readonly string InvalidUserName = "Tên đăng nhập không chính xác";
            public static readonly string InvalidPassWord = "Mật khẩu không chính xác";
            public static readonly string DeActiveAccount = "Tài khoản chưa được active";
            public static readonly string ChangePassword = "Đổi mật khẩu thành công";
            public static readonly string NoDeActiveCurrent = "Không thể huỷ kích hoạt tài khoản mà bạn đang sử dụng";
            public static readonly string NoDeEnableCurrent = "Không thể xoá tài khoản mà bạn đang sử dụng";


            public static readonly string NotExistEnglish = "Account does not exist";
            public static readonly string InvalidUserNameEnglish = "Username is incorrect";
            public static readonly string DeActiveAccountEnglish = "The account has not been activated";
            public static readonly string InvalidPassWordEnglish = "Incorrect password";
            public static readonly string ChangePasswordEnglish = "Change password successfully";
        }

        public static class Customer
        {
            public static readonly string NotExist = "Tài khoản không tồn tại";
            public static readonly string NotEmpty = "Thông tin đăng nhập không được để trống";
            public static readonly string UserNameNotEmpty = "Tên đăng nhập không được để trống";
            public static readonly string FullNameNotEmpty = "Họ tên không được để trống";
            public static readonly string FullNameNotEmptyEnglish = "Fullname is required";
            public static readonly string EmailInvalid = "Email không đúng định dạng";
            public static readonly string UniqueUserName = "Tên đăng nhập đã tồn tại";
            public static readonly string CodeNotEmpty = "Mã không được để trống";
            public static readonly string UniqueCode = "Mã đã tồn tại";
            public static readonly string PassWordNotEmpty = "Mật khẩu không được để trống";
            public static readonly string InvalidUserNamePassWord = "Tên đăng nhập hoặc mật khẩu không chính xác";
            public static readonly string AccountHasBeenBlock = "Tài khoản của bạn đã bị khoá";
            public static readonly string IdentityCardInvalid = "CMND không hợp lệ";
            public static readonly string CurrentPassWordNotEmpty = "Mật khẩu hiện tại không được để trống";
            public static readonly string CurrentPassWordInValid = "Mật khẩu hiện tại không chính xác";
            public static readonly string NewPassWordNotEmpty = "Mật khẩu mới không được để trống";
            public static readonly string UniqueEmail = "Email đã tồn tại";
            public static readonly string EmailNotEmpty = "Email không được để trống";
            public static readonly string EmailNotEmptyEnglish = "Email is required";
            public static readonly string UniquePhone = "Số điện thoại đã tồn tại";
            public static readonly string PhoneNumberNotEmpty = "Số điện thoại không được để trống";
            public static readonly string PhoneNumberNotEmptyEnglish = "Phone number is required";
            public static readonly string AddressNotEmpty = "Địa chỉ không được để trống";
            public static readonly string AddressNotEmptyEnglish = "Address is required";
            public static readonly string RoleNotEmpty = "Vai trò không được để trống";
            public static readonly string InvalidUserName = "Tên đăng nhập không chính xác";
            public static readonly string InvalidPassWord = "Mật khẩu không chính xác";
            public static readonly string DeActiveAccount = "Tài khoản chưa được active";
            public static readonly string IdCardNotEmpty = "Chứng minh nhân dân không được để trống";
            public static readonly string IdCardPlaceNotEmpty = "Nơi cấp chứng minh nhân dân không được để trống";
            public static readonly string UniqueIdCard = "Chứng minh nhân dân đã tồn tại";
            public static readonly string UniqueIdCardEnglish = "Identity card already exists";
            public static readonly string UniqueIdCardPlace= "Nơi cấp chứng minh nhân dân đã tồn tại";
            public static readonly string NotChangeRole = "Không phải là thành viên của Chapter nên không thể đặt chức vụ";
            public static readonly string NotSetFreeMember = "Không thể chọn gói khi loại người dùng là khách vãng lai";
            public static readonly string NotSetPremiumMember = "Vui lòng chọn gói kích hoạt cho loại người dùng thành viên";
            public static readonly string ChapterNotActive = "Chapter bạn chọn đã bị huỷ kích hoạt hoặc đã bị xoá, vui lòng chọn chapter khác";
            public static readonly string ChapterNotActiveEnglish = "The chapter you selected has been deactivated or deleted, please choose another chapter";
            public static readonly string NotAcceptChapter =
                "Tài khoản của bạn chưa được xác nhận vào chapter. Vui lòng đợi xác nhận vào chapter";


            public static readonly string UniquePhoneEnglish = "Phone number already exists";
            public static readonly string UniqueEmailEnglish = "Email already exists";
        }

        public static class Business
        {
            public static readonly string NotExist = "Doanh nghiệp không tồn tại";
            public static readonly string NameNotEmpty = "Tên doanh nghiệp không được để trống";
            public static readonly string UniqueName = "Tên doanh nghiệp đã tồn tại";
            public static readonly string PositionNotEmpty = "Chức vụ không được để trống";
            public static readonly string AddressNotEmpty = "Địa chỉ doanh nghiệp không được để trống";
            public static readonly string TaxCodeNotEmpty = "Mã số thuế không được để trống";
            public static readonly string UniqueTaxCode = "Mã số thuế đã tồn tại";
            public static readonly string NotSubscribeChapter = "Chapter bạn chọn đã tồn tại lĩnh vực của bạn, vui lòng chọn chapter khác";
            public static readonly string NotSubscribeChapterEnglish = "The chapter you selected already exists in your field, please choose another chapter";
            public static readonly string NotChangeFieldOperations = "Lĩnh vực bạn chọn đã tồn tại trong chapter, vui lòng chọn lĩnh vực khác";
            public static readonly string NotChangeFieldOperationsEnglish = "Your chosen field already exists in the chapter, please choose another field";
            public static readonly string NotAcceptChapter = "Trong chapter đã tồn tại lĩnh vực của người dùng";
            public static readonly string NotChange = "Bạn đã yêu cầu thay đổi thông tin, vui lòng đợi duyệt";
            public static readonly string UniqueFieldOperations = "Vui lòng chọn lĩnh vực khác lĩnh vực ban đầu";
            public static readonly string UniqueFieldOperationsEnglish = "Please select a field other than the original field";
            public static readonly string NotChangeEnglish = "You have requested to change the information, please wait for approval";
        }


        public static class Department
		{
            public static readonly string NotExist = "Phòng ban không tồn tại";
			public static readonly string NameNotEmpty = "Tên phòng ban không được để trống";
			public static readonly string CodeNotEmpty = "Mã phòng ban không được để trống";
			public static readonly string UniqueName = "Tên phòng ban đã tồn tại";
			public static readonly string UniqueCode = "Mã phòng ban đã tồn tại";
		}

        public static class Hub
        {
			public static readonly string NotExist = "Hub không tồn tại";
			public static readonly string CenterNotExist = "Trung tâm không tồn tại";
			public static readonly string PONotExist = "Chi nhánh không tồn tại";
            public static readonly string NameNotEmpty = "Tên Hub không được để trống";
            public static readonly string CodeNotEmpty = "Mã Hub không được để trống";
			public static readonly string UniqueName = "Tên Hub đã tồn tại";
			public static readonly string UniqueCode = "Mã Hub đã tồn tại";
        }

        public static class StationHub
        {
            public static readonly string NotExist = "Trạm không tồn tại";
            public static readonly string NotEmpty = "Trạm không được để trống";
        }

		public static class Role
        {
            public static readonly string NotDestroy = "Vai trò đã được sử dụng, không thể xoá";
            public static readonly string NotChange = "Vai trò đã được sử dụng, không thể thay đổi";
            public static readonly string NotExist = "Vai trò không tồn tại";
            public static readonly string NameNotEmpty = "Tên vai trò không được để trống";
            public static readonly string CodeNotEmpty = "Mã vai trò không được để trống";
			public static readonly string UniqueName = "Tên vai trò đã tồn tại";
			public static readonly string UniqueCode = "Mã vai trò đã tồn tại";
            public static readonly string RoleTypeNotEmpty = "Loại không được để trống";
        }

		public static class Transaction
		{
			public static readonly string NotExist = "Quốc gia không tồn tại";
			public static readonly string NameNotEmpty = "Tên quốc gia không được để trống";
			public static readonly string CodeNotEmpty = "Mã quốc gia không được để trống";
			public static readonly string UniqueName = "Tên quốc gia đã tồn tại";
			public static readonly string UniqueCode = "Mã quốc gia đã tồn tại";
		}
        public static class FAQ
        {
            public static readonly string NotExist = "FAQs không tồn tại";
            public static readonly string QuestionNotEmpty = "Câu hỏi không được để trống";
            public static readonly string UniqueQuestion = "Câu hỏi đã tồn tại";
            public static readonly string PriorityNotEmpty = "Thứ tự hiển thị không được để trống";
            public static readonly string UniquePriority = "Thứ tự hiển thị đã tồn tại";
        }

        public static class Expense
        {
            public static readonly string NotDeActive = "Gói đang được sử dụng, không thể huỷ active";
            public static readonly string NotDestroy = "Gói đang được sử dụng, không thể xoá";
            public static readonly string NotExist = "Gói không tồn tại";
            public static readonly string NotActiveOrEnable = "Gói đã bị huỷ kích hoạt hoặc bị xoá, vui lòng chọn gói khác";
            public static readonly string NameNotEmpty = "Tên gói không được để trống";
            public static readonly string CodeNotEmpty = "Mã gói không được để trống";
            public static readonly string UniqueName = "Tên gói đã tồn tại";
            public static readonly string UniqueCode = "Mã gói đã tồn tại";
        }

        public static class Province
		{
			public static readonly string NotExist = "Tỉnh/thành không tồn tại";
			public static readonly string NameNotEmpty = "Tên tỉnh/thành không được để trống";
			public static readonly string CodeNotEmpty = "Mã tỉnh/thành không được để trống";
			public static readonly string UniqueName = "Tên tỉnh/thành đã tồn tại";
			public static readonly string UniqueCode = "Mã tỉnh/thành đã tồn tại";
		}

        public static class Region
        {
            public static readonly string NotExist = "Vùng không tồn tại";
            public static readonly string NameNotEmpty = "Tên vùng không được để trống";
            public static readonly string CodeNotEmpty = "Mã vùng không được để trống";
            public static readonly string UniqueName = "Tên vùng đã tồn tại";
            public static readonly string UniqueCode = "Mã vùng đã tồn tại";
            public static readonly string NotActive = "Vui lòng kích hoạt tỉnh trước khi kích hoạt vùng";
            public static readonly string ProvinceNotActive = "Tỉnh/thành bạn chọn đã bị huỷ kích hoạt hoặc đã bị xoá, vui lòng chọn tỉnh thành khác";
        }

        public static class Chapter
        {
            public static readonly string NotExist = "Chapter không tồn tại";
            public static readonly string NameNotEmpty = "Tên chapter không được để trống";
            public static readonly string CodeNotEmpty = "Mã chapter không được để trống";
            public static readonly string UniqueName = "Tên chapter đã tồn tại";
            public static readonly string UniqueCode = "Mã chapter đã tồn tại";
            public static readonly string NotActive = "Vui lòng kích hoạt vùng trước khi kích hoạt chapter";
            public static readonly string ProvinceNotActive = "Tỉnh/thành bạn chọn đã bị huỷ kích hoạt hoặc đã bị xoá, vui lòng chọn tỉnh thành khác";
            public static readonly string RegionNotActive = "Vùng bạn chọn đã bị huỷ kích hoạt hoặc đã bị xoá, vui lòng chọn vùng khác";
        }

        public static class Opportunity
        {
			public static readonly string NotExist = "Cơ hội k không tồn tại";
			public static readonly string NameNotEmpty = "Tên quận/huyện không được để trống";
			public static readonly string CodeNotEmpty = "Mã quận/huyện không được để trống";
			public static readonly string UniqueName = "Tên quận/huyện đã tồn tại";
			public static readonly string UniqueCode = "Mã quận/huyện đã tồn tại";
		}

		public static class Ward
		{
			public static readonly string NotExist = "Phường/xã không tồn tại";
            public static readonly string WardListNotEmpty = "Chưa chọn quận/huyện";
			public static readonly string NameNotEmpty = "Tên phường/xã không được để trống";
			public static readonly string CodeNotEmpty = "Mã phường/xã không được để trống";
			public static readonly string UniqueName = "Tên phường/xã đã tồn tại";
			public static readonly string UniqueCode = "Mã phường/xã đã tồn tại";
		}

        public static class FaceToFace
        {
            public static readonly string LocationNotEmpty = "Địa điểm gặp mặt không được để trống";
            public static readonly string LocationNotEmptyEnglish = "The meeting place cannot be left blank";
            public static readonly string UniqueLicensePlate = "Biển số đã tồn tại";
            public static readonly string ExchangeTime = "Ngày giờ gặp mặt không được sau ngày giờ hiện tại";
            public static readonly string ExchangeTimeEnglish = "The meeting date and time cannot be after the current date and time";
        }

        public static class Guests
        {
            public static readonly string FullNameNotEmpty = "Họ và tên khách mời không được để trống";
            public static readonly string PhoneNumberNotEmpty = "Số điện thoại khách mời không được để trống";
            public static readonly string EmailNotEmpty = "Email khách mời không được để trống";
            public static readonly string AddressNotEmpty = "Địa chỉ khách mời không được để trống";
            public static readonly string MeetingWhereNotEmpty = "Địa điểm/ Link cuộc họp không được để trống";

        }

        public static class Event
        {
            public static readonly string NotExist = "Sự kiện không tồn tại";
            public static readonly string IsEnd = "Sự kiện đã kết thúc, không thể kích hoạt";
            public static readonly string EndEvent = "Sự kiện đã kết thúc, không thể mở lại";
            public static readonly string NameNotEmpty = "Tên sự kiện không được để trống";
            public static readonly string LinkInformationNotEmpty = "Link thông tin sự kiện không được để trống";
            public static readonly string LinkCheckInNotEmpty = "Link thông tin check-in không được để trống";
            public static readonly string NotDeActive = "Sự kiện đang được diễn ra, đã có thành viên đăng ký. Không thể huỷ kích hoạt";
            public static readonly string NotActive = "Sự kiện đã hết thời gian diễn ra. Không thể kích hoạt";
            public static readonly string NotDeEnabled= "Sự kiện đang được diễn ra, đã có thành viên đăng ký. Không thể xoá";
            public static readonly string NotDeEnd = "Sự kiện đang được diễn ra, đã có thành viên đăng ký. Không thể kết thúc";
            public static readonly string NotDateTimeEvent = "Vui lòng chọn ngày giờ bắt đầu và ngày giờ kết thúc";
            public static readonly string NotDateStartEvent = "Vui lòng chọn ngày bắt đầu";
            public static readonly string NotDateEndEvent = "Vui lòng chọn ngày kết thúc";
            public static readonly string NotTimeStartEvent = "Vui lòng chọn giờ bắt đầu";
            public static readonly string NotTimeEndEvent = "Vui lòng chọn giờ kết thúc";
            public static readonly string NotRegister = "Bạn đã đăng ký sự kiện này, vui lòng chờ Admin duyệt";
            public static readonly string NotRegisterEnglish = "You have registered for this event, please wait for Admin approval";
            public static readonly string IsActive = "Sự kiện đã bị huỷ kích hoạt";
            public static readonly string End = "Sự kiện đã hết hạn";


            public static readonly string FreeMemberNotRegister =
                "Đăng ký trở thành thành viên OBC để được tham gia sự kiện";
            public static readonly string FreeMemberNotRegisterEnglish =
                "Register to become an OCB member to participate in the event";

        }

        public static class Course
        {
            public static readonly string NotExist = "Khoá học không tồn tại";
            public static readonly string IsEnd = "Khoá học đã kết thúc, không thể kích hoạt";
            public static readonly string EndCourse = "Khoá học đã kết thúc, không thể mở lại";
            public static readonly string NameNotEmpty = "Tên khoá học không được để trống";
            public static readonly string TimeNotEmpty = "Thời gian diễn ra không được để trống";
            public static readonly string NotChangeForm = "Khoá học đang được sử dụng, không thể đổi hình thức";
            public static readonly string NotDeActive = "Khoá học đang được diễn ra, đã có thành viên đăng ký. Không thể huỷ kích hoạt";
            public static readonly string NotActive = "Khoá học đã hết thời gian diễn ra. Không thể kích hoạt";
            public static readonly string NotDeEnabled = "Kháo học đang được diễn ra, đã có thành viên đăng ký. Không thể xoá";
            public static readonly string CourseIsActive = "Khoá học đã bị huỷ kích hoạt";
            public static readonly string FreeMemberNotRegister =
                "Đăng ký trở thành thành viên OBC để được tham gia khoá học";
            public static readonly string FreeMemberNotRegisterEnglish =
                "Register to become an OCB member to participate in the course";
            public static readonly string NotRegister = "Bạn đã đăng ký khoá học này, vui lòng chờ Admin duyệt";
            public static readonly string NotRegisterEnglish = "You have registered for this course, please wait for the Admin to approve";
            public static readonly string NotDateTimeEvent = "Vui lòng chọn ngày giờ bắt đầu và ngày giờ kết thúc";
            public static readonly string NotDateStartEvent = "Vui lòng chọn ngày bắt đầu";
            public static readonly string NotDateEndEvent = "Vui lòng chọn ngày kết thúc";
            public static readonly string NotTimeStartEvent = "Vui lòng chọn giờ bắt đầu";
            public static readonly string NotTimeEndEvent = "Vui lòng chọn giờ kết thúc";
        }

        public static class Video
        {
            public static readonly string NotExist = "Video không tồn tại";
            public static readonly string IsEnd = "Video đã kết thúc, không thể kích hoạt";
            public static readonly string EndCourse = "Video đã kết thúc, không thể mở lại";
            public static readonly string NameNotEmpty = "Tên Video không được để trống";
            public static readonly string NotChangeForm = "Video đang được sử dụng, không thể đổi hình thức";
            public static readonly string NotDeActive = "Video đang được diễn ra, đã có thành viên đăng ký. Không thể huỷ kích hoạt";
            public static readonly string NotActive = "Video đã hết thời gian diễn ra. Không thể kích hoạt";
            public static readonly string NotTimeEvent = "Vui lòng chọn ngày bắt đầu và ngày kết thúc";
            public static readonly string NotDeEnabled = "Kháo học đang được diễn ra, đã có thành viên đăng ký. Không thể xoá";
            public static readonly string CourseIsActive = "Video đã bị huỷ kích hoạt";
            public static readonly string FreeMemberNotRegister =
                "Đăng ký trở thành thành viên OBC để được xem Video";
            public static readonly string FreeMemberNotRegisterEnglish =
                "Register to become an OCB member to watch Videos";
            public static readonly string NotRegister = "Bạn đã đăng ký video này, vui lòng chờ Admin duyệt";
            public static readonly string NotRegisterEnglish = "You have subscribed to this video, please wait for Admin to approve";
            public static readonly string TimeNotEmpty = "Thời gian được xem video không được để trống";
        }

        public static class ContentNotify
        {
            public static readonly string AcceptChapter = "Đăng kí thành công Chapter {0} thành công! Vui lòng thanh toán để được kích hoạt tài khoản";
            public static readonly string CancelChapter = "Bạn không đủ điều kiện để tham gia Chapter {0}. Vui lòng kiểm tra thông tin.";
            public static readonly string AcceptPremium = "Chúc mừng bạn đã trở thành thành viên của OBC.";
            public static readonly string ThanksFor = "Bạn nhận được một Lời cảm ơn từ {0}. Bấm để xem chi tiết";
            public static readonly string OpportunityFor = "Bạn nhận được một Cơ hội kinh doanh từ {0}. Bấm để xem chi tiết";
            public static readonly string FaceToFaceFor = "Bạn nhận được có một cuộc hẹn từ {0}. Bấm để xem chi tiết";
            public static readonly string FaceToFaceSuccess = "Cuộc hẹn được đồng ý từ {0}. Bấm để xem chi tiết";
            public static readonly string FaceToFaceCancel = "Cuộc hẹn bị từ chối từ {0}. Bấm để xem chi tiết";
            public static readonly string RegisterEvent = "Đăng kí thành công Sự Kiện {0}. Bấm để xem chi tiết";
            public static readonly string RegisterCourse = "Đăng kí thành công Khoá học {0}. Bấm để xem chi tiết";
            public static readonly string RegisterVideo = "Đăng kí thành công Video {0}. Bấm để xem chi tiết";
            public static readonly string CancelRegisterEvent = "Đăng kí không thành công Sự Kiện {0}. Bấm để xem chi tiết.";
            public static readonly string CancelRegisterCourse = "Đăng kí không thành công Khoá học {0}. Bấm để xem chi tiết";
            public static readonly string CancelRegisterVideo = "Đăng kí không thành công Video {0}. Bấm để xem chi tiết";
            public static readonly string ChangeProfession = "Chúc mừng bạn đã thay đổi ngành nghề {0} thành công. Ngành nghề mới đã được cập nhật ở Thông tin doanh nghiệp.";
            public static readonly string ChangeFieldOperations = "Chúc mừng bạn đã thay đổi lĩnh vực {0} thành công. Lĩnh vực mới đã được cập nhật ở Thông tin doanh nghiệp.";
            public static readonly string CancelChangeProfession = "Ngành nghề của bạn không được chấp nhận. Vui lòng chọn Ngành nghề khác!";
            public static readonly string CancelChangeFieldOperations = "Lĩnh vực của bạn không được chấp nhận. Vui lòng chọn Lĩnh vực khác!";

            public static readonly string AcceptChapterEnglish = "Successfully Registered Chapter {0}! Please pay to activate your account";
            public static readonly string CancelChapterEnglish = "Your  information is not enough to join Chapter {0}. Please check the information.";
            public static readonly string AcceptPremiumEnglish = "Congratulations on becoming a member of OBC";
            public static readonly string ThanksForEnglish = "Thank you For the closed Business - Receive from {0}. Click to view details";
            public static readonly string OpportunityForEnglish = "Referral - Receive from {0}. Click to view details.";
            public static readonly string FaceToFaceForEnglish = "Face  to  Face - Receive from {0}.Click to view details.";
            public static readonly string FaceToFaceSuccessEnglish = "Your Face to Face is approved {0}. Click to view details";
            public static readonly string FaceToFaceCancelEnglish = "Your Face to Face is reject {0}. Click to view details";
            public static readonly string RegisterEventEnglish = "Successfully Registered Event {0}. Click to view details";
            public static readonly string RegisterCourseEnglish = "Successfully Registered Course {0}. Click to view details";
            public static readonly string RegisterVideoEnglish = "Successfully Registered Video {0}. Click to view details";
            public static readonly string CancelRegisterEventEnglish = "Registration failed to Event {0}. Click to view details";
            public static readonly string CancelRegisterCourseEnglish = "Registration failed to Course {0}. Click to view details";
            public static readonly string CancelRegisterVideoEnglish = "Registration failed to Video {0}. Click to view details.";
            public static readonly string ChangeProfessionEnglish = "Congratulations on your successful career change {0}. New career have been updated in Business Information";
            public static readonly string ChangeFieldOperationsEnglish = "Congratulations on your successful field change {0}. New Fields have been updated in Business Information";
            public static readonly string CancelChangeProfessionEnglish = "The career is unacceptable. Please choose another career!";
            public static readonly string CancelChangeFieldOperationsEnglish = "The fields is unacceptable. Please choose another fields!";
        }

        public static class MeetingChapter
        {
            public static readonly string NameNotEmpty = "Tên cuộc họp không được để trống";
            public static readonly string NameNotEmptyEnglish = "The meeting name is required";
            public static readonly string LinkNotEmpty = "Link meeting không được để trống";
            public static readonly string LinkNotEmptyEnglish = "Link meeting is required";
            public static readonly string AddressNotEmpty = "Địa điểm không được để trống";
            public static readonly string AddressNotEmptyEnglish = "Address is required";
            public static readonly string DateEndInCorrect = "Ngày kết thúc không được trước hoặc trùng ngày bắt đầu buổi họp";
            public static readonly string DateEndInCorrectEnglish = "The end date cannot be before or the same as the meeting start date";
            public static readonly string TimeInCorrect = "Thời gian diễn ra không được trước hoặc trùng thời gian hiện tại";
            public static readonly string TimeInCorrectEnglish = "The time of the event cannot be before or the same time as the current time";
        }
    }
}
