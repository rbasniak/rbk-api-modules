using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Email;
using rbkApiModules.Commons.Core.Utilities.Localization;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class DefaultAuthenticationMailService : IAuthenticationMailService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthEmailOptions _mailConfiguration;
    private readonly ILocalizationService _localization;
    private readonly string _url;
    private readonly string _logoHtml = "<img height=\"24\" src=\"cid:mail_logo_01\" alt=\"Logo\" title=\"Logo\" style=\"outline:none;max-height:70px;text-decoration:none;display:block\" />";

    public DefaultAuthenticationMailService(IHttpContextAccessor httpContextAccessor, IOptions<AuthEmailOptions> mailConfiguration, ILocalizationService localization)
    {
        _httpContextAccessor = httpContextAccessor;
        _mailConfiguration = mailConfiguration.Value;
        _localization = localization;
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
            .Replace("{{mainColor}}", _mailConfiguration.EmailData.MainColor)
            .Replace("{{fontColor}}", _mailConfiguration.EmailData.FontColor)
            .Replace("{{systemName}}", _mailConfiguration.EmailData.Logo == null ? _mailConfiguration.Sender.Name  : _logoHtml)
            .Replace("{{activationLink}}", $"{_url}/api/auth/confirm-email?email={receiverEmail}&code={activationCode}");

        var title = $"{_mailConfiguration.Sender.Name} - {_localization.LocalizeString(AuthenticationMessages.Emails.RegistrationConfirmed)}";

        SendEmail(title, textBody, htmlBody, receiverEmail);
    }

    public void SendConfirmationSuccessMail(string receiverName, string receiverEmail)
    {
        var textBody = ReadMailResource("email-confirmed.txt")
            .Replace("{{name}}", receiverName)
            .Replace("{{profileUrl}}", _mailConfiguration.EmailData.AccountDetailsUrl);

        var htmlBody = ReadMailResource("email-confirmed.html")
            .Replace("{{name}}", receiverName)
            .Replace("{{mainColor}}", _mailConfiguration.EmailData.MainColor)
            .Replace("{{fontColor}}", _mailConfiguration.EmailData.FontColor)
            .Replace("{{systemName}}", _mailConfiguration.EmailData.Logo == null ? _mailConfiguration.Sender.Name : _logoHtml)
            .Replace("{{profileUrl}}", _mailConfiguration.EmailData.AccountDetailsUrl);

        var title = $"{_mailConfiguration.Sender.Name} - {_localization.LocalizeString(AuthenticationMessages.Emails.Welcome)} {receiverName}";

        SendEmail(title, textBody, htmlBody, receiverEmail);
    }

    public void SendPasswordResetMail(string receiverName, string receiverEmail, string resetCode)
    {
        var textBody = ReadMailResource("email-reset.txt")
            .Replace("{{name}}", receiverName)
            .Replace("{{resetLink}}", $"{_mailConfiguration.EmailData.PasswordResetUrl}/{resetCode}");

        var htmlBody = ReadMailResource("email-reset.html")
            .Replace("{{name}}", receiverName)
            .Replace("{{mainColor}}", _mailConfiguration.EmailData.MainColor)
            .Replace("{{fontColor}}", _mailConfiguration.EmailData.FontColor)
            .Replace("{{systemName}}", _mailConfiguration.EmailData.Logo == null ? _mailConfiguration.Sender.Name : _logoHtml)
            .Replace("{{resetLink}}", $"{_mailConfiguration.EmailData.PasswordResetUrl}?code={resetCode}");

        var title = $"{_mailConfiguration.Sender.Name} - {_localization.LocalizeString(AuthenticationMessages.Emails.PasswordReset)}";

        SendEmail(title, textBody, htmlBody, receiverEmail);

    }

    public void SendPasswordResetSuccessMail(string receiverName, string receiverEmail)
    {
        var textBody = ReadMailResource("reset-confirmed.txt")
            .Replace("{{name}}", receiverName)
            .Replace("{{profileUrl}}", _mailConfiguration.EmailData.AccountDetailsUrl)
            .Replace("{{suportMail}}", _mailConfiguration.EmailData.SuportEmail);

        var htmlBody = ReadMailResource("reset-confirmed.html")
            .Replace("{{name}}", receiverName)
            .Replace("{{mainColor}}", _mailConfiguration.EmailData.MainColor)
            .Replace("{{fontColor}}", _mailConfiguration.EmailData.FontColor)
            .Replace("{{systemName}}", _mailConfiguration.EmailData.Logo == null ? _mailConfiguration.Sender.Name : _logoHtml)
            .Replace("{{profileUrl}}", _mailConfiguration.EmailData.AccountDetailsUrl)
            .Replace("{{suportMail}}", _mailConfiguration.EmailData.SuportEmail);

        var title = $"{_mailConfiguration.Sender.Name} - {_localization.LocalizeString(AuthenticationMessages.Emails.PasswordSuccessfullyReset)}";

        SendEmail(title, textBody, htmlBody, receiverEmail);
    }

    private void SendEmail(string title, string textBody, string htmlBody, string receiverEmail)
    {
        var credentials = string.IsNullOrEmpty(_mailConfiguration.Sender.Password)
            ? null
            : new NetworkCredential(_mailConfiguration.Sender.Email, _mailConfiguration.Sender.Password);

        EmailHandler.SendEmail(
            smtpHost: _mailConfiguration.Server.SmtpHost,
            enableSSL: _mailConfiguration.Server.SSL,
            port: _mailConfiguration.Server.Port,
            sender: new MailAddress(_mailConfiguration.Sender.Email, _mailConfiguration.Sender.Name),
            receiver: new MailAddress(receiverEmail),
            title: title,
            textBody: textBody,
            htmlBody: htmlBody,
            inlineImages: _mailConfiguration.EmailData.Logo != null
                ? new InlineImage[] { new InlineImage("mail_logo_01", new ContentType("image/png"), _mailConfiguration.EmailData.Logo) }
                : null,
            attachments: null,
            credential: credentials,
            deliveryMethod: _mailConfiguration.TestMode.Enabled ? SmtpDeliveryMethod.SpecifiedPickupDirectory : SmtpDeliveryMethod.Network,
            pickupDirectoryLocation: _mailConfiguration.TestMode.OutputFolder);
    }

    private string ReadMailResource(string resourceName)
    {
        var assembly = Assembly.GetAssembly(GetType());

        var allResources = assembly.GetManifestResourceNames();

        var resource = allResources.First(x => x.EndsWith(resourceName));

        var stream = assembly.GetManifestResourceStream(resource);

        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }
}