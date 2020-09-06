using AspNetCoreApiTemplate.Auditing;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.Models;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Auditing
{
    /// <summary>
    /// Behavior do MediatR para salvamento automático de dados de auditoria
    /// </summary>
    /// <typeparam name="TRequest">Tipo do request (uso automático do MediatR)</typeparam>
    /// <typeparam name="TResponse">Tipo da resposta (uso automático do MediatR)</typeparam>
    public class AuditingPostProcessingBehavior<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : IRequest<BaseResponse>
        where TResponse : BaseResponse
    {
        private readonly string _user;
        private readonly AuditingContext _audidintContext;

        /// <summary>
        /// Construtor padrão
        /// </summary>
        /// <param name="httpContextAccessor">Serviço para acesso ao HttpContext (para pegar o nome do usuário)</param>
        /// <param name="auditingContext">SErviço do banco de dados de auditoria</param>
        public AuditingPostProcessingBehavior(IHttpContextAccessor httpContextAccessor, AuditingContext auditingContext)
        {
            _audidintContext = auditingContext;

            if (httpContextAccessor.HttpContext == null) // This happens in integration tests
            {
                _user = "-";
            }
            else
            {
                _user = httpContextAccessor.HttpContext.User.Identity.Name;
            }
        }

        /// <summary>
        /// Métodos que executa o pos-processamento (uso automático do MediatR)
        /// </summary>
        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Command {request.GetType().DeclaringType.Name.Replace("`1", "")} handled " + (((dynamic)response).IsValid ? "with" : "without") + " success");

            if (response is CommandResponse)
            {
                var commandResponse = response as CommandResponse;

                if (commandResponse.IsValid && commandResponse.EventData != null)
                {
                    if (request is IPassword passwordRequest)
                    {
                        passwordRequest.Password = "********";
                    }

                    var @event = new StoredEvent((response as CommandResponse).EventData.EntityId, request.GetType().Name, JsonConvert.SerializeObject(request), _user);

                    await _audidintContext.AddAsync(@event);
                    await _audidintContext.SaveChangesAsync();
                }
            }
        } 
    }
}
