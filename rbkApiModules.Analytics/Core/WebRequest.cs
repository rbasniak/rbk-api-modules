using System;
using System.Net;

namespace rbkApiModules.Analytics
{
    /// <summary>
    /// Classe para armazenar dados da requisição sendo feita à API
    /// </summary>
    public class WebRequest
    {
        public DateTime Timestamp { get; set; }

        public string Identity { get; set; }

        public string Username { get; set; }

        public IPAddress RemoteIpAddress { get; set; }

        public string Method { get; set; }

        public int Response { get; set; }

        public bool IsHttps { get; set; }

        public string Path { get; set; }

        public string UserAgent { get; set; } 

        public long Duration { get; set; }
    }
}
