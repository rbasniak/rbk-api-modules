using System.Collections.Generic;

namespace rbkApiModules.Infrastructure.Api
{
    public class ApiModuleOptions
    {
        private bool _isProduction = false;
        private bool _useSwagger = true;
        private bool _useHttps = true;

        private readonly List<ApplicationRoute> _routes = new();

        public bool IsProduction => _isProduction;
        public bool UseSwagger => _useSwagger;
        public bool UseHttps => _useHttps;

        public List<ApplicationRoute> Routes => _routes;

        public ApiModuleOptions SetIsProduction(bool isProduction)
        {
            _isProduction = isProduction;
            return this;
        }

        public ApiModuleOptions SetUseSwagger(bool useSagger)
        {
            _useSwagger = useSagger;
            return this;
        }

        public ApiModuleOptions SetUseHttps(bool useHttps)
        {
            _useHttps = useHttps;
            return this;
        }

        public ApiModuleOptions AddRoute(ApplicationRoute route)
        {
            _routes.Add(route);
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
