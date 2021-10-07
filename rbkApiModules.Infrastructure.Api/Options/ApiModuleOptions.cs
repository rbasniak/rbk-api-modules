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
        public bool UsingSwagger => _useSwagger;
        public bool UsingHttps => _useHttps;

        public List<ApplicationRoute> Routes => _routes;

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

        public ApiModuleOptions UseSpaOnRoot()
        {
            HasSpaOnRoot = true;
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
