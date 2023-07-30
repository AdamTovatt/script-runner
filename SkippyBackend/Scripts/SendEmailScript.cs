using ScriptRunner;
using System.Net;
using System.Net.Mail;

namespace CustomScripts
{
    public class SendEmailScript : CompiledScript
    {
        public SendEmailScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script sends an email using Gmail's SMTP server.
        /// </summary>
        /// <param name="from">The sender's email address</param>
        /// <param name="password">The sender's email password</param>
        /// <param name="to">The recipient's email address</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="body">The body of the email</param>
        [ScriptStart]
        public void SendEmailUsingGmailSmtp(string from, string password, string to, string subject, string body)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(from, password);

            MailMessage mailMessage = new MailMessage(from, to);
            mailMessage.Subject = subject;
            mailMessage.Body = body;

            client.Send(mailMessage);
        }
    }
}
