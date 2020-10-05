using System;
using System.IO;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using rbkApiModules.Infrastructure.Api;

namespace rbkApiModules.Demo.Controllers.Abstract
{
    public class UploadProcessingResponse
    {
        public bool Success { get; set; }
        public string Filename { get; set; }
        public KeyValueAccumulator FormData { get; set; }
        public string Error { get; set; }
    }

    public class BaseUploadController : BaseController
    {
        private static readonly FormOptions DefaultFormOptions = new FormOptions();

        public BaseUploadController() : base()
        {
        }

        protected async Task<UploadProcessingResponse> ProcessUploadedFile(int maxWidth)
        {
            var formData = new KeyValueAccumulator();
            string filename = null;
            var error = String.Empty;

            try
            {
                if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                {
                    error = $"Esperado um request to tipo 'multipart', mas o tipo é '{Request.ContentType}'";
                    return new UploadProcessingResponse { Error = error, Success = false };
                }

                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), DefaultFormOptions.MultipartBoundaryLengthLimit);

                var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                var section = reader.ReadNextSectionAsync().Result;

                while (section != null)
                {
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            var extension = Path.GetExtension(contentDisposition.FileName.Value.Replace("\"", "")).ToLower();
                        }
                        else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                        {
                            // Content-Disposition: form-data; name="key"
                            //
                            // value

                            // Do not limit the key name length here because the 
                            // multipart headers length limit is already in effect.
                            var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name);

                            var encoding = GetEncoding(section);

                            using (var streamReader = new StreamReader(section.Body, encoding, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
                            {
                                // The value length limit is enforced by MultipartBodyLengthLimit
                                var value = streamReader.ReadToEndAsync().Result;

                                if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                                {
                                    value = String.Empty;
                                }

                                formData.Append(key.Value, value);

                                if (formData.ValueCount > DefaultFormOptions.ValueCountLimit)
                                {
                                    throw new InvalidDataException($"Form key count limit {DefaultFormOptions.ValueCountLimit} exceeded.");
                                }
                            }
                        }
                    }

                    // Drains any remaining section body that has not been consumed and reads the headers for the next section.
                    section = reader.ReadNextSectionAsync().Result;
                }

                if (String.IsNullOrEmpty(filename))
                {
                    error = "Erro ao processar o arquivo no servidor";

                    return new UploadProcessingResponse { Error = error, Success = false };
                }

                return new UploadProcessingResponse { Success = true, Filename = filename, FormData = formData };
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return new UploadProcessingResponse { Error = error, Success = false };
            }
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF8.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }

    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec says 70 characters is a reasonable limit.
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
            if (string.IsNullOrWhiteSpace(boundary.Value))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary.Value;
        }

        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && string.IsNullOrEmpty(contentDisposition.FileName.ToString())
                   && string.IsNullOrEmpty(contentDisposition.FileNameStar.ToString());
        }

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && (!string.IsNullOrEmpty(contentDisposition.FileName.ToString())
                       || !string.IsNullOrEmpty(contentDisposition.FileNameStar.ToString()));
        }
    }
}
