using System.Collections.Generic;

namespace Data
{
    public class MailConfig
    {
        public Credentials Credentials { get; set; }
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public string From { get; set; }
        public IEnumerable<EmailAddress> Recipients { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}