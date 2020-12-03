using DesignPatternCmsInlupp.Models;
using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DesignPatternCmsInlupp.Services.Logger;

namespace DesignPatternCmsInlupp.Observers
{
    public class NewMail
    {
        public event EventHandler<MailEventargs> OnSendMail;
        public void CatchNewEmail(string email, string transactionType, string message)
        {
            OnSendMail.Invoke(this, new MailEventargs { Email = email, TansactionType = transactionType, Message = message });
        }
        public void SendMail(object sender, MailEventargs e)
        {
            var mailer = new Mailer();
            mailer.SendMail(e.Email, e.TansactionType, e.Message);
        }
    }
}