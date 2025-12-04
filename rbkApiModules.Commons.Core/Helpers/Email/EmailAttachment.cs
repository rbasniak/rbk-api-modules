using System.Net.Mime;

namespace rbkApiModules.Commons.Core.Email;
public class MailAttachment
{
    public ContentType ContentType { get; private set; }

    public string Base64Content { get; private set; }

    public MailAttachment(ContentType contentType, string base64Content)
    {
        ContentType = contentType;
        Base64Content = base64Content;
    }
}
