using Microsoft.AspNetCore.Http;
using rbkApiModules.Utilities.Email;
using rbkApiModules.Utilities.Email.Models;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace rbkApiModules.Authentication
{

    public interface IAuthenticationMailService
    {
        /// <summary>
        /// Envia o e-mail de confirmação para o novo usuário com o link de confirmação do e-mail cadastrado
        /// </summary>
        void SendConfirmationMail(string receiverName, string receiverEmail, string activationCode);

        /// <summary>
        /// Envia o e-mail de sucesso de registro do usuário
        /// </summary>
        void SendConfirmationSuccessMail(string receiverName, string receiverEmail);

        /// <summary>
        /// Envia o e-mail de redefinição de senha com o link de contendo o código para criação de uma nova senha
        /// </summary>
        void SendPasswordResetMail(string receiverName, string receiverEmail, string resetCode);
    }

    public class AuthenticationMailService : IAuthenticationMailService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationMailConfiguration _mailConfiguration;
        private readonly string _url;
        private readonly string _logoHtml = "<img height=\"24\" src=\"cid:mail_logo_01\" alt=\"Logo\" title=\"Logo\" style=\"outline:none;max-height:70px;text-decoration:none;display:block\" />";

        private string ReadMailResource(string resourceName)
        {
            var assembly = Assembly.GetAssembly(GetType());

            var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources." + resourceName);

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public AuthenticationMailService(IHttpContextAccessor httpContextAccessor, AuthenticationMailConfiguration mailConfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _mailConfiguration = mailConfiguration;
            _url = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://" +
                $"{_httpContextAccessor.HttpContext?.Request.Host}" +
                $"{_httpContextAccessor.HttpContext?.Request.PathBase}";
        }

        public void SendConfirmationMail(string receiverName, string receiverEmail, string activationCode)
        {
            var textBody = ReadMailResource("email-confirmation.txt")
                .Replace("{{name}}", receiverName)
                .Replace("{{activationLink}}", $"{_url}/auth/confirm?email={receiverEmail}&code={activationCode}");

            var htmlBody = ReadMailResource("email-confirmation.html")
                .Replace("{{name}}", receiverName)
                .Replace("{{mainColor}}", _mailConfiguration.MainColor)
                .Replace("{{fontColor}}", _mailConfiguration.FontColor)
                .Replace("{{systemName}}", _mailConfiguration.B64Logo == null ? _mailConfiguration.SenderName : _logoHtml)
                .Replace("{{activationLink}}", $"{_url}/auth/confirm?email={receiverEmail}&code={activationCode}");

            var credentials = string.IsNullOrEmpty(_mailConfiguration.SenderPassword) 
                ? null 
                : new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword);

            EmailHandler.SendEmail(
                _mailConfiguration.SMTPHost,
                _mailConfiguration.SSL,
                _mailConfiguration.Port,
                new MailAddress(_mailConfiguration.SenderMail, _mailConfiguration.SenderName),
                new MailAddress(receiverEmail),
                $"{_mailConfiguration.SenderName} - Confirmação de registro",
                textBody,
                htmlBody,
                _mailConfiguration.B64Logo != null
                    ? new InlineImage[] { new InlineImage("mail_logo_01", new ContentType("image/png"), _mailConfiguration.B64Logo) }
                    : null,
                null,
                credentials);
        }

        public void SendConfirmationSuccessMail(string receiverName, string receiverEmail)
        {
            var textBody = ReadMailResource("email-confirmed.txt")
                .Replace("{{name}}", receiverName)
                .Replace("{{profileUrl}}", _mailConfiguration.AccountDetailsUrl);

            var htmlBody = ReadMailResource("email-confirmed.html")
                .Replace("{{name}}", receiverName)
                .Replace("{{mainColor}}", _mailConfiguration.MainColor)
                .Replace("{{fontColor}}", _mailConfiguration.FontColor)
                .Replace("{{systemName}}", _mailConfiguration.B64Logo == null ? _mailConfiguration.SenderName : _logoHtml)
                .Replace("{{profileUrl}}", _mailConfiguration.AccountDetailsUrl);

            var credentials = string.IsNullOrEmpty(_mailConfiguration.SenderPassword)
                ? null
                : new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword);

            EmailHandler.SendEmail(
                _mailConfiguration.SMTPHost,
                _mailConfiguration.SSL,
                _mailConfiguration.Port,
                new MailAddress(_mailConfiguration.SenderMail, _mailConfiguration.SenderName),
                new MailAddress(receiverEmail),
                $"{_mailConfiguration.SenderName} - Bem-vindo {receiverName}",
                textBody,
                htmlBody,
                _mailConfiguration.B64Logo != null
                    ? new InlineImage[] { new InlineImage("mail_logo_01", new ContentType("image/png"), _mailConfiguration.B64Logo) }
                    : null,
                null,
                credentials);
        }

        public void SendPasswordResetMail(string receiverName, string receiverEmail, string resetCode)
        {
            var textBody = ReadMailResource("email-reset.txt")
                .Replace("{{name}}", receiverName)
                .Replace("{{resetLink}}", $"{_mailConfiguration.PasswordResetUrl}/{resetCode}");

            var htmlBody = ReadMailResource("email-reset.html")
                .Replace("{{name}}", receiverName)
                .Replace("{{mainColor}}", _mailConfiguration.MainColor)
                .Replace("{{fontColor}}", _mailConfiguration.FontColor)
                .Replace("{{systemName}}", _mailConfiguration.B64Logo == null ? _mailConfiguration.SenderName : _logoHtml)
                .Replace("{{resetLink}}", $"{_mailConfiguration.PasswordResetUrl}/{resetCode}");

            var credentials = string.IsNullOrEmpty(_mailConfiguration.SenderPassword)
                ? null
                : new NetworkCredential(_mailConfiguration.SenderMail, _mailConfiguration.SenderPassword);

            EmailHandler.SendEmail(
                _mailConfiguration.SMTPHost,
                _mailConfiguration.SSL,
                _mailConfiguration.Port,
                new MailAddress(_mailConfiguration.SenderMail, _mailConfiguration.SenderName),
                new MailAddress(receiverEmail),
                $"{_mailConfiguration.SenderName} - Redefinição de senha",
                textBody,
                htmlBody,
                _mailConfiguration.B64Logo != null
                    ? new InlineImage[] { new InlineImage("mail_logo_01", new ContentType("image/png"), _mailConfiguration.B64Logo) }
                    : null,
                null,
                credentials);
        }
    }
}
