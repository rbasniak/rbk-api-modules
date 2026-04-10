using System.Net.Mime;

namespace rbkApiModules.Commons.Core.Email;
public class InlineImage
{
    public string ContentId { get; private set; }

    public ContentType ContentType { get; private set; }

    public string Base64Content { get; private set; }

    public InlineImage(string contentId, ContentType contentType, string base64Content)
    {
        ContentId = contentId;
        ContentType = contentType;
        Base64Content = base64Content;
    }
}