using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Numerics;

namespace rbkApiModules.Analytics
{
    /// <summary>
    /// Extensões para auxiliar na obtenção de dados do request
    /// </summary>
    public static class ServerSideExtensions
    {
        const string cookieName = "SSA_Identity";
        private const string StrFormat = "000000000000000000000000000000000000000";

        /// <summary>
        /// Método para pegar a identificação do usuário
        /// </summary>
        public static string UserIdentity(this HttpContext context)
        {
            string identity = context.User?.Identity?.Name;

            if (!context.Request.Cookies.ContainsKey(cookieName))
            {
                if (string.IsNullOrWhiteSpace(identity))
                {
                    identity = context.Request.Cookies.ContainsKey("ai_user")
                                ? context.Request.Cookies["ai_user"]
                                : context.Connection.Id;
                }

                if (!context.Response.HasStarted)
                    context.Response.Cookies.Append("identity", identity);
            }
            else
            {
                identity = context.Request.Cookies[cookieName];
            }

            return identity;
        }

        /// <summary>
        /// Método converter um IP em string
        /// </summary>
        public static string ToFullDecimalString(this IPAddress ip)
        {
            return (new BigInteger(ip.MapToIPv6().GetAddressBytes().Reverse().ToArray())).ToString(StrFormat);
        }

        /// <summary>
        /// Extensão para registrar e configurar os serviços de analytics na API
        /// </summary>
        public static AnalyticBuilder UseServerSideAnalytics(this IApplicationBuilder app, IAnalyticStore repository)
        {
            var builder = new AnalyticBuilder(repository);
            app.Use(builder.Run);
            return builder;
        }
    }
}
