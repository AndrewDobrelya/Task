using System;
using System.Web.Mvc;
using System.Net.Mail;

namespace IndividualTaskManagement.ExceptionFilter
{
    public class SmtpExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled &&
                    filterContext.Exception is SmtpException)
            {
               var val = ((SmtpException)filterContext.Exception).Message;
                filterContext.Result = new ViewResult
                {
                    ViewName = "MailError",
                    ViewData = new ViewDataDictionary<string>(val)
                };
                filterContext.ExceptionHandled = true;
            }
        }
    }
}