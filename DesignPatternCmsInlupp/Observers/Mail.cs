using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Observers
{
    public class Mail
    {
        public event EventHandler<MailEventargs> SendMail;
        public void CatchEmail(string to, string subject, string message)
        {
            SendMail.Invoke(this, new MailEventargs{ To = to, Subject = subject, Message = message });
        }
    }
}