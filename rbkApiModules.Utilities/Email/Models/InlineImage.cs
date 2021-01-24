using System.Net.Mime;

namespace rbkApiModules.Utilities.Email.Models
{
    /// <summary>
    /// Classe contendo dados de uma imagem a ser adicionada no corpo de um email
    /// </summary>
    public class InlineImage
    {
        /// <summary>
        /// Identificador da imagem
        /// </summary>
        public string ContentId { get; private set; }

        /// <summary>
        /// Tipo MIME da imagem
        /// </summary>
        public ContentType ContentType { get; private set; }

        /// <summary>
        /// Texto em Base64 contendo a imagem
        /// </summary>
        public string Base64Content { get; private set; }

        /// <summary>
        /// Inicia uma nova inst�ncia da classe <see cref="InlineImage" />.
        /// </summary>
        /// <param name="contentId">Identificador da imagem</param>
        /// <param name="contentType">Tipo MIME da imagem</param>
        /// <param name="base64Content">Texto em Base64 contendo a imagem</param>
        public InlineImage(string contentId, ContentType contentType, string base64Content)
        {
            ContentId = contentId;
            ContentType = contentType;
            Base64Content = base64Content;
        }
    }
}