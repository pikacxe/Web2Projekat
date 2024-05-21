using Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiUserData.Settings
{
    internal class MailClientSettings : ISettingsValidator
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public string? SenderEmail { get; set; }

        public bool isValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Host)
                    || string.IsNullOrWhiteSpace(Username)
                    || string.IsNullOrWhiteSpace(Password)
                    || string.IsNullOrEmpty(SenderEmail))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
