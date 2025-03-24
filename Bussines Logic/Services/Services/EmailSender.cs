
namespace Bussines_Logic.Services.Services
{
	public class EmailSender : IEmailSender
	{
		private readonly MailSettings emailSender;
		//private readonly IWebHostEnvironment _webHostEnvironment;
		public EmailSender(IOptions<MailSettings> options/*, IWebHostEnvironment webHostEnvironment*/)
		{
			emailSender = options.Value;
			//_webHostEnvironment = webHostEnvironment;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			MailMessage message = new()
			{
				From = new MailAddress(emailSender.Email!, emailSender.DisplayName),
				Body = htmlMessage,
				Subject = subject,
				IsBodyHtml = true
			};

			SmtpClient smtpClient = new(emailSender.Host)
			{
				Port = emailSender.Port,
				Credentials = new NetworkCredential(emailSender.Email, emailSender.password),
				EnableSsl = true
			};

			message.To.Add("mahmoudtark556@gmail.com");
			await smtpClient.SendMailAsync(message);

			smtpClient.Dispose();
		}
	}
}
