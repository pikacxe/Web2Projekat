using System.Net;
using System.Net.Mail;
using TaxiUserData.Settings;

namespace TaxiUserData.Helpers
{
    internal class MailHelper
    {
        private const string mailBody = """
                <h3>You driver account has been verified</h3>
                <p>Dear <strong>customer</strong>,</br></br>
                Your driver account has been verified. You can now start accepting rides. We hope to see you frequently on our platform.
                </br></br>
                Drive safe and best regards,
                </br>
                Your TaxiMockup team
                </p>
                """;
        private readonly MailClientSettings _settings;

        public MailHelper(MailClientSettings settings)
        {
            _settings = settings;
        }

        public async Task SendVerificationMailAsync(string usernEmail, CancellationToken cancellationToken = default)
        {

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            MailMessage message = new MailMessage();
            message.From = new MailAddress(_settings.SenderEmail ?? throw new ArgumentException("Mail Settings not set"));
            message.To.Add(new MailAddress(usernEmail));
            message.IsBodyHtml = true;
            message.Subject = "Account verification process complete";
            message.Body = mailBody;


            await client.SendMailAsync(message,cancellationToken);
        }

    }
}
