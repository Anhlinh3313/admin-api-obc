using System;
namespace Core.Entity.Entities
{
    public class EmailRecipient
    {
        public EmailRecipient(int id, string email, string name, string codeResetPassWord, string phoneNumber,string content)
        {
            Id = id;
            Email = email;
            Name = name;
            CodeResetPassWord = codeResetPassWord;
            PhoneNumber = phoneNumber;
            Content = content;
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string CodeResetPassWord { get; set; }
        public string PhoneNumber { get; set; }
        public string Content { get; set; }
    }

    public class DataResponse
    {
        public DataResponse(string otp, DateTime createdWhen)
        {
            OTP = otp;
            CreatedWhen = createdWhen;
        }
        public string OTP { get; set; }
        public DateTime CreatedWhen { get; set; }
    }
}
