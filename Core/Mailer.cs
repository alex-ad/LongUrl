using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace LongUrl.Core
{
	public static class Mailer
	{
		public static void Send(string email, string token)
		{
			var emailMessage = new MimeMessage();

			emailMessage.From.Add(new MailboxAddress("LongURL", "longurl.info@gmail.com"));
			emailMessage.To.Add(new MailboxAddress("", email));
			emailMessage.Subject = "LongURL API Token";
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = $"Hello. You token is: {token}"
			};

			using var client = new SmtpClient();
			client.SendAsync(emailMessage);
		}
	}
}
