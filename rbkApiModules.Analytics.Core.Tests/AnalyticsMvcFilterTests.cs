using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Microsoft.AspNetCore.Routing.Patterns;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Controllers;
using rbkApiModules.Infrastructure.Api;
using System.Reflection;
using System;
using System.Linq.Expressions;
using rbkApiModules.Utilities.Testing;
using Shouldly;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class AnalyticsMvcFilterTests
    {
        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsMvcFilter))]
        public void Should_Not_Throw_On_OnActionExecuted() // It's empty, just for security and coverage
        {
            var filter = new AnalyticsMvcFilter();
            var actionContext = CreateActionExecutedContextFilter<TestControllerWithArea>(x => x.TestActionWithArea(), out var httpContext);

            Should.NotThrow(() => filter.OnActionExecuted(actionContext));
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsMvcFilter))]
        public void Should_Use_Action_Name_Instead_Of_Path_When_It_Exists()
        {
            var filter = new AnalyticsMvcFilter();
            var actionContext = CreateActionExecutingContextFilterWithRoutedEndpoint<TestControllerWithArea>(x => x.TestActionWithArea(), out var httpContext);

            filter.OnActionExecuting(actionContext);

            httpContext.Items.TryGetValue(AnalyticsMvcFilter.LOG_DATA_PATH, out var path).ShouldBe(true);
            path.ShouldBe("/api/my-action-route");
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsMvcFilter))]
        public void Should_Use_Url_Path_When_Action_Name_Does_Not_Exist()
        {
            var filter = new AnalyticsMvcFilter();
            var actionContext = CreateActionExecutingContextFilterWithoutRoutedEndpoint<TestControllerWithArea>(x => x.TestActionWithArea(), out var httpContext);

            filter.OnActionExecuting(actionContext);

            httpContext.Items.TryGetValue(AnalyticsMvcFilter.LOG_DATA_PATH, out var path).ShouldBe(true);
            path.ShouldBe("/url-path");
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsMvcFilter))]
        public void Should_Prioritize_Action_Area_Instead_Of_Controller()
        {
            var filter = new AnalyticsMvcFilter();
            var actionContext = CreateActionExecutingContextFilterWithRoutedEndpoint<TestControllerWithArea>(x => x.TestActionWithArea(), out var httpContext);

            filter.OnActionExecuting(actionContext);

            httpContext.Items.TryGetValue(AnalyticsMvcFilter.LOG_DATA_AREA, out var area).ShouldBe(true);
            area.ShouldBe("my-custom-area");
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsMvcFilter))]
        public void Should_Use_Controller_Area_When_Action_Does_Not_Have_It()
        {
            var filter = new AnalyticsMvcFilter();
            var actionContext = CreateActionExecutingContextFilterWithRoutedEndpoint<TestControllerWithArea>(x => x.TestActionWithoutArea(), out var httpContext);

            filter.OnActionExecuting(actionContext);

            httpContext.Items.TryGetValue(AnalyticsMvcFilter.LOG_DATA_AREA, out var area).ShouldBe(true);
            area.ShouldBe("my-controller-area");
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsMvcFilter))]
        public void Should_Use_Action_Area_When_Controller_Does_Not_Have_It()
        {
            var filter = new AnalyticsMvcFilter();
            var actionContext = CreateActionExecutingContextFilterWithRoutedEndpoint<TestControllerWithoutArea>(x => x.TestActionWithArea(), out var httpContext);

            filter.OnActionExecuting(actionContext);

            httpContext.Items.TryGetValue(AnalyticsMvcFilter.LOG_DATA_AREA, out var area).ShouldBe(true);
            area.ShouldBe("my-custom-area");
        }

        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(AnalyticsMvcFilter))]
        public void Should_Have_No_Data_When_Neither_Controller_And_Action_Does_Not_Have_It()
        {
            var filter = new AnalyticsMvcFilter();
            var actionContext = CreateActionExecutingContextFilterWithRoutedEndpoint<TestControllerWithoutArea>(x => x.TestActionWithoutArea(), out var httpContext);

            filter.OnActionExecuting(actionContext);

            httpContext.Items.TryGetValue(AnalyticsMvcFilter.LOG_DATA_AREA, out var area).ShouldBe(true);
            area.ShouldBe("not defined");
        }

        private ActionExecutingContext CreateActionExecutingContextFilterWithRoutedEndpoint<T>(Expression<Action<T>> expression, out HttpContext httpContext)
        {
            var routedEndpoint = CreateRoutedEndpoint("/api/my-action-route");

            var endpointFeature = new Mock<IEndpointFeature>();
            endpointFeature.Setup(x => x.Endpoint).Returns(routedEndpoint);

            var featureCollection = new Mock<IFeatureCollection>();
            featureCollection.Setup(x => x.Get<IEndpointFeature>()).Returns(endpointFeature.Object);

            httpContext = Mock.Of<HttpContext>();
            Mock.Get(httpContext).Setup(s => s.Request.Path).Returns("/api/endpoint");
            Mock.Get(httpContext).Setup(s => s.Features).Returns(featureCollection.Object);
            Mock.Get(httpContext).Setup(s => s.Items).Returns(new Dictionary<object, object?>());

            var controllerActionDescriptor = new ControllerActionDescriptor();
            controllerActionDescriptor.ActionName = nameof(TestControllerWithArea.TestActionWithoutArea);
            controllerActionDescriptor.ControllerTypeInfo = typeof(T).GetTypeInfo();
            controllerActionDescriptor.MethodInfo = MethodInfoHelper.GetMethodInfo<T>(expression);

            var actionContext = new ActionContext(
                httpContext,
                Mock.Of<RouteData>(),
                controllerActionDescriptor,
                new ModelStateDictionary()
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );

            return actionExecutingContext;
        }

        private ActionExecutingContext CreateActionExecutingContextFilterWithoutRoutedEndpoint<T>(Expression<Action<T>> expression, out HttpContext httpContext)
        {
            var routedEndpoint = CreateEndpoint("/api/my-action-route");

            var endpointFeature = new Mock<IEndpointFeature>();
            endpointFeature.Setup(x => x.Endpoint).Returns(routedEndpoint);

            var featureCollection = new Mock<IFeatureCollection>();
            featureCollection.Setup(x => x.Get<IEndpointFeature>()).Returns(endpointFeature.Object);

            httpContext = Mock.Of<HttpContext>();
            Mock.Get(httpContext).Setup(s => s.Request.Path).Returns("/url-path");
            Mock.Get(httpContext).Setup(s => s.Features).Returns(featureCollection.Object);
            Mock.Get(httpContext).Setup(s => s.Items).Returns(new Dictionary<object, object?>());

            var controllerActionDescriptor = new ControllerActionDescriptor();
            controllerActionDescriptor.ActionName = nameof(TestControllerWithArea.TestActionWithoutArea);
            controllerActionDescriptor.ControllerTypeInfo = typeof(T).GetTypeInfo();
            controllerActionDescriptor.MethodInfo = MethodInfoHelper.GetMethodInfo<T>(expression);

            var routedData = new EndpointFeature();

            var actionContext = new ActionContext(
                httpContext,
                Mock.Of<RouteData>(),
                controllerActionDescriptor,
                new ModelStateDictionary()
            );

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );

            return actionExecutingContext;
        }

        private ActionExecutedContext CreateActionExecutedContextFilter<T>(Expression<Action<T>> expression, out HttpContext httpContext)
        {
            var routedEndpoint = CreateRoutedEndpoint("/api/my-action-route");

            var endpointFeature = new Mock<IEndpointFeature>();
            endpointFeature.Setup(x => x.Endpoint).Returns(routedEndpoint);

            var featureCollection = new Mock<IFeatureCollection>();
            featureCollection.Setup(x => x.Get<IEndpointFeature>()).Returns(endpointFeature.Object);

            httpContext = Mock.Of<HttpContext>();
            Mock.Get(httpContext).Setup(s => s.Request.Path).Returns("/api/endpoint");
            Mock.Get(httpContext).Setup(s => s.Features).Returns(featureCollection.Object);
            Mock.Get(httpContext).Setup(s => s.Items).Returns(new Dictionary<object, object?>());

            var controllerActionDescriptor = new ControllerActionDescriptor();
            controllerActionDescriptor.ActionName = nameof(TestControllerWithArea.TestActionWithoutArea);
            controllerActionDescriptor.ControllerTypeInfo = typeof(T).GetTypeInfo();
            controllerActionDescriptor.MethodInfo = MethodInfoHelper.GetMethodInfo<T>(expression);

            var actionContext = new ActionContext(
                httpContext,
                Mock.Of<RouteData>(),
                controllerActionDescriptor,
                new ModelStateDictionary()
            );

            var actionExecutedContext = new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                Mock.Of<Controller>()
            );

            return actionExecutedContext;
        }

        private RouteEndpoint CreateRoutedEndpoint(string template, object defaults = null, object constraints = null, int? order = null)
        {
            return new RouteEndpoint(
                (context) => Task.CompletedTask,
                RoutePatternFactory.Parse(template, defaults, constraints),
                order ?? 0,
                EndpointMetadataCollection.Empty,
                "endpoint: " + template);
        }

        private Endpoint CreateEndpoint(string template, object defaults = null, object constraints = null, int? order = null)
        {
            return new Endpoint((context) => Task.CompletedTask, EndpointMetadataCollection.Empty, "");
        }
    } 

    [ApplicationArea("my-controller-area")]
    public class TestControllerWithArea : ControllerBase
    {
        [HttpGet]
        public ActionResult TestActionWithoutArea()
        {
            return Ok();
        }

        [HttpGet]
        [ApplicationArea("my-custom-area")]
        public ActionResult TestActionWithArea()
        {
            return Ok();
        }
    }

    public class TestControllerWithoutArea : ControllerBase
    {
        [HttpGet]
        public ActionResult TestActionWithoutArea()
        {
            return Ok();
        }

        [HttpGet]
        [ApplicationArea("my-custom-area")]
        public ActionResult TestActionWithArea()
        {
            return Ok();
        }
    }

    public class EndpointFeature : IEndpointFeature
    {
        public Endpoint Endpoint { get; set; }
    }
}
