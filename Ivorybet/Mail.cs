using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Ivorybet
{

    public class Mail
    {
        private SmtpClient xSender = new SmtpClient();
        private MailMessage xMsg = new MailMessage();
        public String MailFrom = "";
        public String[] MailTo;
        public String Subject = "";
        public String Body = "";
        public string Host = "";

        public Mail()
        {

        }

        public Mail(string nHost)
        {
            Host = nHost;
        }

        public Mail(string nHost, string nMailFrom, string[] nMailTo)
        {
            Host = nHost;
            MailFrom = nMailFrom;
            MailTo = nMailTo;
        }

        public void AddAttachmentFile(string nFilePath)
        {
            try
            {
                Attachment attachmentFile = new Attachment(nFilePath);
                xMsg.Attachments.Add(attachmentFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddImageLink(string nContentId, string nImgRealPath)
        {
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString("<img src=cid:" + nContentId + ">", null, "text/html");
            LinkedResource img = new LinkedResource(nImgRealPath);
            img.ContentId = nContentId;
            htmlView.LinkedResources.Add(img);
            xMsg.AlternateViews.Add(htmlView);
        }

        public void SendAsync()
        {

            try
            {
                //Send Mail
                xMsg.From = new MailAddress(MailFrom);
                for (int i = 0; i < MailTo.Length; i++)
                {
                    xMsg.To.Add(new MailAddress(MailTo[i]));
                }

                xMsg.Subject = Subject;
                xMsg.Body = Body;
                xMsg.IsBodyHtml = true;
                //   xMsg.BodyEncoding = System.Text.Encoding.UTF8;
                xSender.Host = Host;
                //xSender.Timeout = 60;

                xSender.SendAsync(xMsg, "IBC Error Message Report!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Send()
        {

            try
            {
                //Send Mail
                xMsg.From = new MailAddress(MailFrom);
                for (int i = 0; i < MailTo.Length; i++)
                {
                    xMsg.To.Add(new MailAddress(MailTo[i]));
                }

                xMsg.Subject = Subject;
                xMsg.Body = Body;
                xMsg.IsBodyHtml = true;
                //   xMsg.BodyEncoding = System.Text.Encoding.UTF8;
                xSender.Host = Host;
                //xSender.Timeout = 60;

                xSender.Send(xMsg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
