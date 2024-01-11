using System.Net.Mail;
using System.Net;

namespace BidMarket.Services
{
    public class MailSender
    {
        private string _secretCode = Guid.NewGuid().ToString();
        public string SecretCode => _secretCode;
        public async Task SendCodeAsync(string reciever) 
        {
            var body = "<html><body><h3>There is your activation code</h3>" +
            $"<br/>{_secretCode}</body></html>";

            await MailTemplateSend(reciever, body);
        }
        public async Task SendDisapproveAsync(string reciever, string lotName, string reason)
        {
            var body = $"<html><body><h3>Sorry, your lot {lotName} will be disabled, because {reason}</h3>" +
            $"</body></html>";

            await MailTemplateSend(reciever, body);
        }
        public async Task SendWinAsync(string reciever, string lotName)
        {
            var body = $"<html><body><h3>Congrats! you win lot - {lotName}</h3>" +
            $"</body></html>";

            await MailTemplateSend(reciever, body);
        }
        public async Task MailTemplateSend(string reciever, string body)
        {
            string fromMail = "nikawak228@gmail.com";
            string fromPassword = "wtzsxtxxxlyxbcfz";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Hello user!";
            message.To.Add(new MailAddress(reciever));
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
    }
}
