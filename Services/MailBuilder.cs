using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using Data;

namespace Services
{
    public class MailBuilder
    {
        private Configuration _configuration;
        private readonly Dictionary<string,object>  _paramList = new Dictionary<string, object>();
        public MailBuilder(Configuration configuration)
        {
            _configuration = configuration;
            
        }

        public Dictionary<string, object> Params
        {
            get { return _paramList; }
        }

        public MailMessage Build(string hostname, string ip, int port)
        {
            MailMessage message = new MailMessage();;
            GenerateParameters(hostname, ip, port);
            message.From = new MailAddress(_configuration.MailConfiguration.SendFrom);
            AddRecipients(message);
            message.Subject = ParseText(_configuration.MailConfiguration.Subject);
            message.Body = ParseText(_configuration.MailConfiguration.Body);

            return message;
        }

        private void AddRecipients(MailMessage message)
        {
            foreach (var recipient in _configuration.MailConfiguration.Recipients)
            {
                message.To.Add(new MailAddress(recipient));
            }
        }

        private string ParseText(string messageSubject)
        {
            foreach (var param in _paramList)
            {
                string replace = "{" + param.Key + "}";
                messageSubject = messageSubject.Replace(replace, param.Value.ToString());
            }
            return messageSubject;
        }

        private void GenerateParameters(string hostname, string ip, int port)
        {
            _paramList.Add("sendfrom", _configuration.MailConfiguration.SendFrom);
            _paramList.Add("timeout", _configuration.TimeOut);
            _paramList.Add("hostname", hostname);
            _paramList.Add("computername", Environment.MachineName);
            _paramList.Add("ip", ip);
            _paramList.Add("port", port);
            _paramList.Add("appname", Assembly.GetExecutingAssembly().GetName().Name);
            _paramList.Add("appversion", new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version);
        }


        public void AddParam(string name, object value)
        {
            _paramList.Add(name,value);
        }
    }
}