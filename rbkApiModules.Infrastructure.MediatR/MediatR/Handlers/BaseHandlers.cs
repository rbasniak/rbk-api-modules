using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Commons;
using System;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Infrastructure.MediatR.Abstract
{
    public abstract class BaseHandler<TCommand, TResponse> where TCommand : IRequest<TResponse> where TResponse : BaseResponse
    {
        protected IHttpContextAccessor _httpContextAccessor;

        public BaseHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void StoreDiagnosticsData(TCommand request, Exception exception)
        {
            var diagnosticsStore = _httpContextAccessor?.HttpContext?.RequestServices?.GetService<IDiagnosticsModuleStore>();

            if (diagnosticsStore != null)
            {
                var exceptionData = new DiagnosticsEntry(_httpContextAccessor.HttpContext, request.GetType().FullName, exception, request);
                diagnosticsStore.StoreData(exceptionData);
            }
        }
    }
}
