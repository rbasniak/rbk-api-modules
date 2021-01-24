using System.Net.Mime;

namespace rbkApiModules.Utilities.Email.Models
{
    /// <summary>
    /// Classe contendo os dados de um anexo a ser adicionado a um email
    /// </summary>
    public class MailAttachment
    {
        /// <summary>
        /// Tipo MIME do anexo
        /// </summary>
        public ContentType ContentType { get; private set; }

        /// <summary>
        /// Texto em Base64 contendo o anexo
        /// </summary>
        public string Base64Content { get; private set; }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MailAttachment" />.
        /// </summary>
        /// <param name="contentType">Tipo MIME do anexo</param>
        /// <param name="base64Content">Texto em Base64 contendo o anexo</param>
        public MailAttachment(ContentType contentType, string base64Content)
        {
            ContentType = contentType;
            Base64Content = base64Content;
        }
    }
}
