using System.Net;

namespace rbkApiModules.Utilities
{
    public class HttpResponse<T>
    {
        public HttpResponse(HttpStatusCode status, T data)
        {
            Status = status;
            Data = data;
        }

        public HttpResponse(HttpStatusCode status, string[] errors)
        {
            Status = status;
            Errors = errors;
        }

        public T Data { get; }
        public bool Success => Status == HttpStatusCode.OK || Status == HttpStatusCode.NoContent;
        public HttpStatusCode Status { get; }
        public string[] Errors { get; }
    }
}
