using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Runtime.CompilerServices;

namespace TaxiUserData
{
    public class MailHelper : IDisposable
    {
        private SmtpClient _smtpClient;

        public MailHelper(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }
        public async Task SendVerificationMailAsync(string email)
        {
            // Development mail service
            await _smtpClient.SendMailAsync("info@taximockup.com", email, "Driver account verified", "Hello dear customer, your account has been verified! You can now start accepting rides!");
        }
        public void Dispose()
        {
            _smtpClient.Dispose();
        }
    }
}
