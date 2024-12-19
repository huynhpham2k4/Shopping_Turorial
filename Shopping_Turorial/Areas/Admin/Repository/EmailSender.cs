using Shopping_Tutorial.Areas.Admin.Repository;
using System.Net.Mail;
using System.Net;

namespace Shopping_Tutorial.Areas.Admin.Repository
{
	public class EmailSender : IEmailSender
	{
		public Task SendEmailAsync(string email, string subject, string message)
		{
			var client = new SmtpClient("smtp.gmail.com", 587)// thuong co 2 cong 465 va 587 // dung 485  se bao mat hon
			{
				EnableSsl = true, //bật bảo mật
				UseDefaultCredentials = false,// khong su dung khoa redencial
				Credentials = new NetworkCredential("phamngochuynh2k4@gmail.com", "pvlgwpvdpwnfkseo")
			};

			return client.SendMailAsync(
				new MailMessage(from: "phamngochuynh2k4@gmail.com",
								to: email,
								subject,
								message
								));
		}
	}
}
