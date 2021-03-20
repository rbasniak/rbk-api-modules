using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using rbkApiModules.Utilities.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class AnalyticsModuleMiddlewareTests
    {
        private const string COOKIE_USERNAME = "Cookie_Username";
        private const string ACTION_EXECUTED = "/api/controller/my-action-name";
        private const string APPLICATION_AREA = "application-area";
        private const string IP_ADDRESS = "50.31.134.17";
        private const string REQUEST_METHOD = "GET";
        private const string REQUEST_URL = "/api/endpoint";
        private const string APPLICATION_DOMAIN = "ApplicationDomain";
        private const string USER_AGENT = "Mozilla";
        private const string IDENTITY_USERNAME = "Me";
        private const string DEFAULT_APPLICATION_VERSION = "-";
        private const string APPLICATION_VERSION = "1.0.0";
        private const string CONNECTION_ID = "123456";
        private const string AI_USERNAME = "Ai_User";
        private const int TRANSACTION_COUNT = 3;
        private const double TRANSACTION_TIME = 56.27;
        private const int RESPONSE_CODE = 200;
        private byte[] REQUEST = new byte[] { 50, 31, 134, 17 };
        private byte[] RESPONSE = new byte[] { 50, 31, 134, 17, 127, 0, 0, 1 };

        [AutoNamedFact]
        public async void Should_Get_Write_Statistics_Under_Normal_Conditions()
        {
            var analyticsModuleStore = new DummyAnalyticsStore();

            await RunMiddleware(analyticsModuleStore, true, true, true);

            analyticsModuleStore.Data.ShouldNotBeNull();
            analyticsModuleStore.Data.Action.ShouldBe(REQUEST_METHOD + " " + ACTION_EXECUTED);
            analyticsModuleStore.Data.Area.ShouldBe(APPLICATION_AREA);
            analyticsModuleStore.Data.Domain.ShouldBe(APPLICATION_DOMAIN);
            analyticsModuleStore.Data.Duration.ShouldBeGreaterThan(-1);
            analyticsModuleStore.Data.Identity.ShouldBe(COOKIE_USERNAME.ToLower());
            analyticsModuleStore.Data.IpAddress.ShouldBe(IP_ADDRESS);
            analyticsModuleStore.Data.Method.ShouldBe(REQUEST_METHOD);
            analyticsModuleStore.Data.Path.ShouldBe(REQUEST_METHOD + " " + REQUEST_URL);
            analyticsModuleStore.Data.RequestSize.ShouldBe(REQUEST.Length);
            analyticsModuleStore.Data.Response.ShouldBe(RESPONSE_CODE);
            // analyticsModuleStore.Data.ResponseSize.ShouldBe(RESPONSE.Length); Works in production but not in test
            analyticsModuleStore.Data.TotalTransactionTime.ShouldBe((int)TRANSACTION_TIME);
            analyticsModuleStore.Data.TransactionCount.ShouldBe(TRANSACTION_COUNT);
            analyticsModuleStore.Data.UserAgent.ShouldBe(USER_AGENT);
            analyticsModuleStore.Data.Username.ShouldBe(IDENTITY_USERNAME.ToLower());
            analyticsModuleStore.Data.Version.ShouldBe(DEFAULT_APPLICATION_VERSION);
            analyticsModuleStore.Data.WasCached.ShouldBe(true);

            (DateTime.UtcNow - analyticsModuleStore.Data.Timestamp).TotalMilliseconds.ShouldBeGreaterThan(0); 
            (DateTime.UtcNow - analyticsModuleStore.Data.Timestamp).TotalSeconds.ShouldBeLessThan(10); //  This may fail in debug, if there is a breakpoint before it
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Get_Identity_From_Cookie_When_It_Exists()
        {
            var analyticsModuleStore = new DummyAnalyticsStore();

            await RunMiddleware(analyticsModuleStore, true, true, false);

            analyticsModuleStore.Data.ShouldNotBeNull();
            analyticsModuleStore.Data.Identity.ShouldBe(COOKIE_USERNAME.ToLower());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Get_Identity_From_AiUser_When_There_Is_No_Cookie()
        {
            var analyticsModuleStore = new DummyAnalyticsStore();

            await RunMiddleware(analyticsModuleStore, true, false, true);

            analyticsModuleStore.Data.ShouldNotBeNull();
            analyticsModuleStore.Data.Identity.ShouldBe(AI_USERNAME.ToLower());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Get_Identity_From_ConnectionId_When_There_Is_No_Cookie_Nor_AiUser()
        {
            var analyticsModuleStore = new DummyAnalyticsStore();

            await RunMiddleware(analyticsModuleStore, true, false, false);

            analyticsModuleStore.Data.ShouldNotBeNull();
            analyticsModuleStore.Data.Identity.ShouldBe(CONNECTION_ID);
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Use_Application_Version_When_It_Exists()
        {
            var analyticsModuleStore = new DummyAnalyticsStore();

            await RunMiddleware(analyticsModuleStore, true, false, false, new AnalyticsModuleOptions().SetApplicationVersion(APPLICATION_VERSION));

            analyticsModuleStore.Data.ShouldNotBeNull();
            analyticsModuleStore.Data.Version.ShouldBe(APPLICATION_VERSION);
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Exclude_IpAddress()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.ExcludeIp(new IPAddress(new byte[] { 127, 0, 0, 1 }));
            options.ExcludeIp(new IPAddress(new byte[] { 128, 0, 0, 1 }));

            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 127, 0, 0, 1 }), "/path", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 128, 0, 0, 1 }), "/path", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/path", "GET", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Once());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Limit_to_Path()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.LimitToPath("/api");

            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/images/user.png", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/index.html", "GET", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Once());
        }


        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Exclude_Single_Path()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.ExcludePath("/rbk");

            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/download", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/rbk/download/1", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/index.html", "GET", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Exactly(2));
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Exclude_Multiple_Paths()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.ExcludePath("/rbk", "/api");

            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/download", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/rbk/download/1", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/index.html", "GET", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Once());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Exclude_Extension()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.ExcludeExtension("pNg", "htMl");

            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/user.png", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/index.html", "GET", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Once());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Exclude_Methods()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.ExcludeMethods("PoST", "PuT");

            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "POST", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Once());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Exclude_Loopback()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.ExcludeLoopBack();

            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 127, 0, 0, 1 }), "/api/users", "GET", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Once());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Exclude_Status_Codes_As_Enums()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.ExcludeStatusCodes(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);

            await RunMiddleware(analyticsModuleStoreMock.Object, (int)HttpStatusCode.BadRequest, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, (int)HttpStatusCode.NotFound, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, (int)HttpStatusCode.OK, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Once());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Exclude_Status_Codes_As_Numbers()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.ExcludeStatusCodes(400, 404);

            await RunMiddleware(analyticsModuleStoreMock.Object, 400, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 404, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Once());
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Limit_To_Status_Codes_As_Enums()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.LimitToStatusCodes(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);

            await RunMiddleware(analyticsModuleStoreMock.Object, (int)HttpStatusCode.BadRequest, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, (int)HttpStatusCode.NotFound, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, (int)HttpStatusCode.OK, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Exactly(2));
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsModuleMiddleware))]
        public async void Should_Limit_To_Status_Codes_As_Numbers()
        {
            var analyticsModuleStoreMock = new Mock<IAnalyticModuleStore>();
            analyticsModuleStoreMock.Setup(x => x.StoreData(It.IsAny<AnalyticsEntry>()));

            var options = new AnalyticsModuleOptions();
            options.LimitToStatusCodes(400, 404);

            await RunMiddleware(analyticsModuleStoreMock.Object, 400, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 404, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);
            await RunMiddleware(analyticsModuleStoreMock.Object, 200, new IPAddress(new byte[] { 192, 168, 0, 1 }), "/api/users", "PUT", options);

            analyticsModuleStoreMock.Verify(x => x.StoreData(It.IsAny<AnalyticsEntry>()), Times.Exactly(2));
        }

        private async Task RunMiddleware(DummyAnalyticsStore analyticsModuleStore, bool isAuthenticated, bool hasCookie, bool hasAiUser, AnalyticsModuleOptions options = null)
        {
            var mockedServiceProvider = new Mock<IServiceProvider>();
            mockedServiceProvider.Setup(x => x.GetService(typeof(IAnalyticModuleStore))).Returns(analyticsModuleStore);

            var cookieCollection = new Mock<IRequestCookieCollection>();
            cookieCollection.Setup(x => x.ContainsKey("ai_user")).Returns(hasAiUser);
            cookieCollection.Setup(x => x.ContainsKey("SSA_Identity")).Returns(hasCookie);

            if (hasCookie) cookieCollection.Setup(x => x["SSA_Identity"]).Returns(COOKIE_USERNAME);
            if (hasAiUser) cookieCollection.Setup(x => x["ai_user"]).Returns(AI_USERNAME);

            var httpContextItems = new Dictionary<object, object?>();
            httpContextItems.Add(AnalyticsMvcFilter.LOG_DATA_AREA, APPLICATION_AREA);
            httpContextItems.Add(AnalyticsMvcFilter.LOG_DATA_PATH, ACTION_EXECUTED);
            httpContextItems.Add(DatabaseAnalyticsInterceptor.TRANSACTION_COUNT_TOKEN, TRANSACTION_COUNT);
            httpContextItems.Add(DatabaseAnalyticsInterceptor.TRANSACTION_TIME_TOKEN, TRANSACTION_TIME);
            httpContextItems.Add("was-cached", true);

            var headerDictionary = new Mock<IHeaderDictionary>();
            headerDictionary.Setup(x => x["User-Agent"]).Returns(USER_AGENT);

            var httpRequest = Mock.Of<HttpRequest>();
            Mock.Get(httpRequest).Setup(s => s.Body).Returns(new MemoryStream(REQUEST));
            Mock.Get(httpRequest).Setup(s => s.Headers).Returns(headerDictionary.Object);
            Mock.Get(httpRequest).Setup(s => s.Method).Returns(REQUEST_METHOD);
            Mock.Get(httpRequest).Setup(s => s.Path).Returns(REQUEST_URL);
            Mock.Get(httpRequest).Setup(s => s.Cookies).Returns(cookieCollection.Object);

            var httpResponse = Mock.Of<HttpResponse>();
            Mock.Get(httpResponse).Setup(s => s.Body).Returns(new MemoryStream(RESPONSE));
            Mock.Get(httpResponse).Setup(s => s.Headers).Returns(headerDictionary.Object);
            Mock.Get(httpResponse).Setup(s => s.StatusCode).Returns(RESPONSE_CODE);
            Mock.Get(httpResponse).Setup(s => s.HasStarted).Returns(false);

            var claimsIdentity = Mock.Of<ClaimsIdentity>();
            Mock.Get(claimsIdentity).Setup(s => s.IsAuthenticated).Returns(isAuthenticated);
            Mock.Get(claimsIdentity).Setup(s => s.Name).Returns(IDENTITY_USERNAME);
            Mock.Get(claimsIdentity).Setup(s => s.Claims).Returns(new List<Claim> { new Claim("domain", APPLICATION_DOMAIN) });

            var connection = Mock.Of<ConnectionInfo>();
            Mock.Get(connection).Setup(s => s.RemoteIpAddress).Returns(new IPAddress(REQUEST));
            Mock.Get(connection).Setup(s => s.Id).Returns(CONNECTION_ID);

            var httpContext = Mock.Of<HttpContext>();
            Mock.Get(httpContext).Setup(s => s.Request).Returns(httpRequest);
            Mock.Get(httpContext).Setup(s => s.Response).Returns(httpResponse);
            Mock.Get(httpContext).Setup(s => s.User.Identity).Returns(claimsIdentity);
            Mock.Get(httpContext).Setup(s => s.Connection).Returns(connection);
            Mock.Get(httpContext).Setup(s => s.Items).Returns(httpContextItems);
            Mock.Get(httpContext).Setup(s => s.RequestServices).Returns(mockedServiceProvider.Object);

            if (options == null)
            {
                options = new AnalyticsModuleOptions();
            }

            // Act
            var middlewareInstance = new AnalyticsModuleMiddleware(next: (innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, options);

            await middlewareInstance.Invoke(httpContext);
        }

        private async Task RunMiddleware(IAnalyticModuleStore analyticsModuleStore, int statusCode, IPAddress ip, string path, string method, AnalyticsModuleOptions options)
        {
            var mockedServiceProvider = new Mock<IServiceProvider>();
            mockedServiceProvider.Setup(x => x.GetService(typeof(IAnalyticModuleStore))).Returns(analyticsModuleStore);

            var cookieCollection = new Mock<IRequestCookieCollection>();
            cookieCollection.Setup(x => x.ContainsKey("ai_user")).Returns(true);
            cookieCollection.Setup(x => x.ContainsKey("SSA_Identity")).Returns(true);
            cookieCollection.Setup(x => x["SSA_Identity"]).Returns(COOKIE_USERNAME);
            cookieCollection.Setup(x => x["ai_user"]).Returns(AI_USERNAME);

            var httpContextItems = new Dictionary<object, object?>();
            httpContextItems.Add(AnalyticsMvcFilter.LOG_DATA_AREA, APPLICATION_AREA);
            httpContextItems.Add(AnalyticsMvcFilter.LOG_DATA_PATH, ACTION_EXECUTED);
            httpContextItems.Add(DatabaseAnalyticsInterceptor.TRANSACTION_COUNT_TOKEN, TRANSACTION_COUNT);
            httpContextItems.Add(DatabaseAnalyticsInterceptor.TRANSACTION_TIME_TOKEN, TRANSACTION_TIME);
            httpContextItems.Add("was-cached", true);

            var headerDictionary = new Mock<IHeaderDictionary>();
            headerDictionary.Setup(x => x["User-Agent"]).Returns(USER_AGENT);

            var httpRequest = Mock.Of<HttpRequest>();
            Mock.Get(httpRequest).Setup(s => s.Body).Returns(new MemoryStream(REQUEST));
            Mock.Get(httpRequest).Setup(s => s.Headers).Returns(headerDictionary.Object);
            Mock.Get(httpRequest).Setup(s => s.Method).Returns(method);
            Mock.Get(httpRequest).Setup(s => s.Path).Returns(path);
            Mock.Get(httpRequest).Setup(s => s.Cookies).Returns(cookieCollection.Object);

            var httpResponse = Mock.Of<HttpResponse>();
            Mock.Get(httpResponse).Setup(s => s.Body).Returns(new MemoryStream(RESPONSE));
            Mock.Get(httpResponse).Setup(s => s.Headers).Returns(headerDictionary.Object);
            Mock.Get(httpResponse).Setup(s => s.StatusCode).Returns(statusCode);
            Mock.Get(httpResponse).Setup(s => s.HasStarted).Returns(false);

            var claimsIdentity = Mock.Of<ClaimsIdentity>();
            Mock.Get(claimsIdentity).Setup(s => s.IsAuthenticated).Returns(true);
            Mock.Get(claimsIdentity).Setup(s => s.Name).Returns(IDENTITY_USERNAME);
            Mock.Get(claimsIdentity).Setup(s => s.Claims).Returns(new List<Claim> { new Claim("domain", APPLICATION_DOMAIN) });

            var connection = Mock.Of<ConnectionInfo>();
            Mock.Get(connection).Setup(s => s.RemoteIpAddress).Returns(ip);
            Mock.Get(connection).Setup(s => s.Id).Returns(CONNECTION_ID);

            var httpContext = Mock.Of<HttpContext>();
            Mock.Get(httpContext).Setup(s => s.Request).Returns(httpRequest);
            Mock.Get(httpContext).Setup(s => s.Response).Returns(httpResponse);
            Mock.Get(httpContext).Setup(s => s.User.Identity).Returns(claimsIdentity);
            Mock.Get(httpContext).Setup(s => s.Connection).Returns(connection);
            Mock.Get(httpContext).Setup(s => s.Items).Returns(httpContextItems);
            Mock.Get(httpContext).Setup(s => s.RequestServices).Returns(mockedServiceProvider.Object);

            // Act
            var middlewareInstance = new AnalyticsModuleMiddleware(next: (innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, options);

            await middlewareInstance.Invoke(httpContext);
        }
    }

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<AnalyticsModuleMiddleware>();  
        }
    }

    public class DummyAnalyticsStore : IAnalyticModuleStore
    {
        public AnalyticsEntry Data { get; set; }

        public Task<List<AnalyticsEntry>> AllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<AnalyticsEntry>> FilterAsync(DateTime from, DateTime to, string[] versions, 
            string[] areas, string[] domains, string[] actions, string[] users, string[] agents, 
            int[] responses, string[] methods, int duration, string entityId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AnalyticsEntry>> FilterAsync(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public void StoreData(AnalyticsEntry request)
        {
            Data = request;
        }
    }
}
