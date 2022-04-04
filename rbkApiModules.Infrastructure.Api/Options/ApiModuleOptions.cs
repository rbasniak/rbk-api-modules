using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;

namespace rbkApiModules.Infrastructure.Api
{
    public class ApiModuleOptions
    {
        private bool _isProduction = false;
        private bool _useSwagger = true;
        private bool _useHttps = false;
        private string _defaultCorsPolicy = null;

        private readonly List<ApplicationRoute> _routes = new();
        private Action<IEndpointRouteBuilder> _hubMappingAction;

        public bool IsProduction => _isProduction;
        public bool UsingSwagger => _useSwagger;
        public bool UsingHttps => _useHttps;
        public string DefaultCorsPolicy => _defaultCorsPolicy;

        public List<ApplicationRoute> Routes => _routes;
        public Action<IEndpointRouteBuilder> HubMappingAction => _hubMappingAction;

        public bool HasSpaOnRoot { get; internal set; }

        public ApiModuleOptions SetEnvironment(bool isProduction)
        {
            _isProduction = isProduction;
            return this;
        }

        public ApiModuleOptions UseSwagger()
        {
            _useSwagger = true;
            return this;
        }

        public ApiModuleOptions UseHttps()
        {
            _useHttps = true;
            return this;
        }

        public ApiModuleOptions AddRoute(string pathString, string indexFilePath)
        {
            _routes.Add(new ApplicationRoute(pathString, indexFilePath));
            return this;
        }

        public ApiModuleOptions MappingSignalRHubs(Action<IEndpointRouteBuilder> action)
        {
            _hubMappingAction = action;
            return this;
        }

        public ApiModuleOptions UseSpaOnRoot()
        {
            HasSpaOnRoot = true;
            return this;
        }

        public ApiModuleOptions SetDefaultCorsPolicy(string policy)
        {
            _defaultCorsPolicy = policy;
            return this;
        }
    }

    public class ApplicationRoute
    {
        public string PathString { get; private set; }

        public string IndexFilePath { get; private set; }

        public ApplicationRoute(string pathString, string indexFilePath)
        {
            PathString = pathString;
            IndexFilePath = indexFilePath;
        }
    }
}
