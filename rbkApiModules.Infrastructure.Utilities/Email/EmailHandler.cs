using rbkApiModules.Infrastructure.Utilities.Email.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;

namespace rbkApiModules.Infrastructure.Utilities.Email
{
    /// <summary>
    /// Classe responsável pelo envio de emails
    /// </summary>
    public class EmailHandler
    {
        /// <summary>
        /// Método responsável pelo envio de e-mails
        /// </summary>
        /// <param name="smtpHost">Nome ou endereço IP do servidor para envio de emails</param>
        /// <param name="enableSSL">Verdadeiro se o servidor usa Secure Sockets Layer (SSL) para encripitar a conexão</param>
        /// <param name="port">Número da porta de comunicação com o servidor</param>
        /// <param name="sender">Endereço de email e nome do remetente</param>
        /// <param name="reciever">Endereço de email do destinatário</param>
        /// <param name="title">Título de email</param>
        /// <param name="textBody">Corpo do email em texto</param>
        /// <param name="htmlBody">Corpo do email em HTML</param>
        /// <param name="inlineImages">Imagens para o corpo email</param>
        /// <param name="attachments">Anexos para o email</param>
        /// <param name="credential">Credencial para autenticar o remetente no servido</param>
        public static void SendEmail(
            string smtpHost,
            bool enableSSL,
            int port,
            MailAddress sender,
            MailAddress reciever,
            string title,
            string textBody = null,
            string htmlBody = null,
            InlineImage[] inlineImages = null,
            MailAttachment[] attachments = null,
            NetworkCredential credential = null)
        {
            var client = new SmtpClient(smtpHost)
            {
                UseDefaultCredentials = false,
                EnableSsl = enableSSL,
                Port = port,
                Credentials = credential,
                Timeout = 15000 // 15 segundos
            };

            var mailMessage = new MailMessage(
                sender,
                reciever)
            {
                Subject = title,
                IsBodyHtml = false,
                Body = textBody
            };

            if (UnityTestEnviromentChecker.OnTestEnviroment)
            {
                var path = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + @"\mail_test";
                Directory.CreateDirectory(path);
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = path;
            }

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
}
