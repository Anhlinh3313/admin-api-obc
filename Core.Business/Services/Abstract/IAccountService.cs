using System.Threading.Tasks;
using Core.Business.ViewModels.Accounts;
using Core.Entity.Entities;
using Core.Infrastructure.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Core.Business.Services.Abstract
{
    public interface IAccountService
    {
        //Task<dynamic> CreateAccount(CreateAccountViewModel model);
        //Task<dynamic> UpdateAccount(UpdateAccountViewModel model); 
        //Task<dynamic> ChangePassWord(ChangePassWordViewModel model);
        //Task<dynamic> DeleteAccount(BasicViewModel model);
        //dynamic GetAccountInfo(int id);
        //dynamic GetAccountList();
        Task<JsonResult> SignIn(SignInViewModel model);
        Task<JsonResult> SignInMobile(SignInMobileViewModel model);
        JsonResult SignUpMobile(SignUpMobileViewModel model);
        User VerifyToken(int userId);
        //ResponseViewModel UpdateUserSystem(UpdateAccountViewModel model);
        //ResponseViewModel GetListUserByUserTypeId(int userTypeId);
        //ResponseViewModel CreateUser(CreateAccountViewModel viewModel);
        JsonResult CheckCodeValidChangePassword(ForgotPasswordViewModel model);
        JsonResult SendMailForgotPassword(ForgotPasswordViewModel model);
        JsonResult CheckCodeValidChangePasswordMobile(ForgotPasswordMobileViewModel model);
        JsonResult SendMailForgotPasswordMobile(ForgotPasswordMobileViewModel model);
        JsonResult SendMailOTP(ForgotPasswordViewModel model);
        JsonResult SendOTPByPhoneNumberRegister(string phoneNumber, string language);
        Task<JsonResult> SendOTPByPhoneNumberForgotPass(string phoneNumber, string language);
        Task<JsonResult> ForgotPasswordMobile(ChangePassWordViewModel model);
        Task<JsonResult> ChangePasswordMobile(ChangePassWordViewModel model, int userId);
        Task<JsonResult> ForgotPasswordWeb(ChangePassWordViewModel model);
        Task<JsonResult> ChangePasswordWeb(ChangePassWordViewModel model);
        JsonResult CheckUniqueEmailAndPhone(string email, string phoneNumber, string language);
        bool SendOTPSOAPViettel(string phoneNumber, string content);
        JsonResult SendMailGuests(int guestsId, string content, string body);
        JsonResult SendMailCustomer(int customerId, string content, string body);

        string SendEmail(dynamic sendEmailOptions, EmailRecipient emailRecipient, string subject);
    }
}
