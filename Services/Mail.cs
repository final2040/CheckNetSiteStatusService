using System;
using System.Collections.Generic;
using System.Net;

namespace Services
{
    public class Mail
    {
        public SmtpClientWraper SmtpClient { get; set; }
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtmlMessage { get; set; }
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public NetworkCredential SmtpCredentials { get; set; }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }

    public class SmtpConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
    }
}