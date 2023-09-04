namespace Core.Business.Services.Models
{
    public class SendMail
    {
        public SendMail()
        {
        }

        public string MailFrom { get; set; }
        public string MailFromDisplayName { get; set; }
        public string PassWordMailFrom { get; set; }
        public string Website { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Path { get; set; }
    }
}
