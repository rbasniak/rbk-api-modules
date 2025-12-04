using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace rbkApiModules.Commons.Core.Email;
public class EmailHandler
{
    public static void SendEmail(
        string smtpHost,
        bool enableSSL,
        int port,
        MailAddress sender,
        MailAddress receiver,
        string title,
        string textBody = null,
        string htmlBody = null,
        InlineImage[] inlineImages = null,
        MailAttachment[] attachments = null,
        NetworkCredential credential = null,
        SmtpDeliveryMethod deliveryMethod = SmtpDeliveryMethod.Network,
        string pickupDirectoryLocation = null)
    {
        var client = new SmtpClient(smtpHost);
        client.PickupDirectoryLocation = pickupDirectoryLocation;
        client.DeliveryMethod = deliveryMethod;
        client.UseDefaultCredentials = false;
        client.EnableSsl = deliveryMethod == SmtpDeliveryMethod.Network ? enableSSL : false;
        client.Port = port;
        client.Credentials = credential;
        client.Timeout = 15000; // 15 segundos

        var mailMessage = new MailMessage(
            sender,
            receiver)
        {
            Subject = title,
            IsBodyHtml = false,
            Body = textBody,
            SubjectEncoding = Encoding.Unicode
        }; 

        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                mailMessage.Attachments.Add(
                    new Attachment(new MemoryStream(Convert.FromBase64String(attachment.Base64Content)), attachment.ContentType)
                );
            }
        }

        if (inlineImages != null && htmlBody != null)
        {
            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, new ContentType("text/html"));

            foreach (var inlineImage in inlineImages)
            {
                htmlView.LinkedResources.Add(
                    new LinkedResource(new MemoryStream(Convert.FromBase64String(inlineImage.Base64Content)), inlineImage.ContentType)
                    {
                        ContentId = inlineImage.ContentId
                    });
            }

            mailMessage.AlternateViews.Add(htmlView);
        }
        else if (htmlBody != null)
        {
            mailMessage.AlternateViews
                .Add(AlternateView.CreateAlternateViewFromString(htmlBody, new ContentType("text/html")));
        }

        client.Send(mailMessage);
    }
}