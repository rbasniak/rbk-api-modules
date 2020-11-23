using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.Models;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Collections.ObjectModel;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace rbkApiModules.Infrastructure.Api.Tests
{
    public class BaseControllerTests
    {
        private ControllerContext PrepareContext(bool mapper, bool mediatr, bool memoryCache)
        {
            var mockedHttpContext = new Mock<HttpContext>();
            var mockedServiceProvider = new Mock<IServiceProvider>();

            if (mapper)
            {
                mockedServiceProvider.Setup(x => x.GetService(typeof(IMapper))).Returns(PrepareAutoMapper());
            }

            if (mediatr)
            {
                mockedServiceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(PrepareMediatrContext());
            }

            if  (memoryCache)
            {
                mockedServiceProvider.Setup(x => x.GetService(typeof(IMemoryCache))).Returns(PrepareMemoryCacheContext());
            }

            var mockedRequestServices = mockedHttpContext.Setup(x => x.RequestServices).Returns(mockedServiceProvider.Object);

            var context = new ControllerContext();

            context.HttpContext = mockedHttpContext.Object;

            return context;
        }

        private IMapper PrepareAutoMapper()        
        {
            var profile = new TestMappings();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            var mapper = new Mapper(configuration);

            return mapper;
        }

        private IMediator PrepareMediatrContext()
        {
            var mockedMediatr = new Mock<IMediator>();
            mockedMediatr.Setup(x => x.Send(It.IsAny<IRequest<CommandResponse>>(), default)).Returns(Task.FromResult(new CommandResponse
            {
                Result = new SomeOtherSampleClass()
            }));

            return mockedMediatr.Object;
        }

        private IMemoryCache PrepareMemoryCacheContext()
        {
            var mockedEntry = new Mock<ICacheEntry>();
            mockedEntry.Setup(x => x.Dispose());

            var mockedCache = new Mock<IMemoryCache>();
            mockedCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(mockedEntry.Object);
            mockedCache.Setup(x => x.Remove(It.IsAny<object>()));

            return mockedCache.Object;
        }

        [Fact]
        public void Should_Return_400_When_There_Are_Validation_Errors_On_a_Response_With_Result()
        {
            var controller = new TestController<CommandResponse>();
            var response = controller.ValidationErrorWithResult();

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ActionResult<SampleClassDto>>();
            response.Result.ShouldBeOfType<BadRequestObjectResult>();

            var result = response.Result as BadRequestObjectResult;
            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBeOfType<ReadOnlyCollection<string>>(); 

            var output = result.Value as ReadOnlyCollection<string>;
            output.Count.ShouldBe(2);

            output[0].ShouldBe("Validation error 1");
            output[1].ShouldBe("Validation error 2");
        }


        [Fact]
        public void Should_Return_400_When_There_Are_Validation_Errors_On_a_Response_Without_Result()
        {
            var controller = new TestController<CommandResponse>();
            var response = controller.ValidationErrorWithoutResult();

            var t1 = response.GetType().FullName;

            response.ShouldNotBeNull();
            response.ShouldBeOfType<BadRequestObjectResult>();

            var result = response as BadRequestObjectResult;
            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBeOfType<ReadOnlyCollection<string>>();

            var output = result.Value as ReadOnlyCollection<string>;
            output.Count.ShouldBe(2);

            output[0].ShouldBe("Validation error 1");
            output[1].ShouldBe("Validation error 2");
        }

        [Fact]
        public void Should_Return_500_When_There_Are_Errors_On_a_Response_With_Result()
        {
            var controller = new TestController<CommandResponse>();
            var response = controller.FatalErrorWithResult();

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ActionResult<SampleClassDto>>();
            response.Result.ShouldBeOfType<ObjectResult>();

            var result = response.Result as ObjectResult;
            result.StatusCode.ShouldBe(500);
            result.Value.ShouldBeOfType<ReadOnlyCollection<string>>();

            var output = result.Value as ReadOnlyCollection<string>;
            output.Count.ShouldBe(1);

            output[0].ShouldBe("Fatal error 1");
        }

        [Fact]
        public void Should_Return_500_When_There_Are_Validation_Errors_On_a_Response_Without_Result()
        {
            var controller = new TestController<CommandResponse>();
            var response = controller.FatalErrorWithoutResult();

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ObjectResult>();

            var result = response as ObjectResult;
            result.StatusCode.ShouldBe(500);
            result.Value.ShouldBeOfType<ReadOnlyCollection<string>>();

            var output = result.Value as ReadOnlyCollection<string>;
            output.Count.ShouldBe(1);

            output[0].ShouldBe("Fatal error 1");  
        }

        [Fact]
        public void Should_Return_200_When_On_a_Response_Without_Result()
        {
            var controller = new TestController<CommandResponse>();
            var response = controller.OkWithoutResult();
            
            response.ShouldNotBeNull();
            response.ShouldBeOfType<OkResult>();

            var result = response as OkResult;
            result.StatusCode.ShouldBe(200);
        }

        [Fact]
        public void Should_Return_200_When_On_a_Response_With_Result_And_Mapping()
        {
            var controller = new TestController<CommandResponse>();
            controller.ControllerContext = PrepareContext(true, false, false);
            
            var response = controller.OkWithResultAndMapping();
            
            response.ShouldNotBeNull();
            response.ShouldBeOfType<ActionResult<SampleClassDto>>();

            var result = response.Result as OkObjectResult;
            
            result.StatusCode.ShouldBe(200);
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<SampleClassDto>();
        }

        [Fact]
        public void Should_Return_200_When_On_a_Response_With_Result_And_No_Mapping()
        {
            var controller = new TestController<CommandResponse>();
            controller.ControllerContext = PrepareContext(true, false, false);

            var response = (ActionResult<SomeOtherSampleClass>)controller.OkWithResultAndNoMapping();

            response.ShouldNotBeNull();

            var result = response.Result as OkObjectResult;

            result.StatusCode.ShouldBe(200);
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<SomeOtherSampleClass>();
        }

        [Fact]
        public void Should_Return_200_When_On_a_Response_With_Result_Cache_And_Mapping()
        {
            var controller = new TestController<CommandResponse>();
            controller.ControllerContext = PrepareContext(true, true, true);

            var response = controller.OkWithResulCacheAndMapping();

            response.ShouldNotBeNull();
            response.ShouldBeOfType<ActionResult<SampleClassDto>>();

            var result = response.Result as OkObjectResult;

            result.StatusCode.ShouldBe(200);
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<SampleClassDto>();
        }

        [Fact]
        public void Should_Return_200_When_On_a_Response_With_Result_Cache_And_No_Mapping()
        {
            var controller = new TestController<CommandResponse>();
            controller.ControllerContext = PrepareContext(false, true, true);

            var response = (ActionResult<SomeOtherSampleClass>)controller.OkWithResultCacheAndNoMapping();

            response.ShouldNotBeNull();

            var result = response.Result as OkObjectResult;

            result.StatusCode.ShouldBe(200);
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<SomeOtherSampleClass>();
        }

        [Fact]
        public async Task Should_MediatR_Setter_Ok()
        {
            var controller = new TestController<CommandResponse>();
            controller.ControllerContext = PrepareContext(false, true, false);

            var response = (ActionResult<SomeOtherSampleClass>)(await controller.OkWithMediatR());

            response.ShouldNotBeNull();

            var result = response.Result as OkObjectResult;

            result.StatusCode.ShouldBe(200);
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<SomeOtherSampleClass>();
        }

        [Fact]
        public void Has_Authorize_Attribute()
        {
            var attributes = (IList<AuthorizeAttribute>)typeof(BaseController).GetCustomAttributes(typeof(AuthorizeAttribute), false);

            attributes.Count.ShouldBe(1);
        }
    }

    public class TestMappings: Profile
    {
        public TestMappings()
        {
            CreateMap<SampleClass, SampleClassDto>();
        }
    }

    public class TestController<T> : BaseController where T: BaseResponse
    {
        public ActionResult<SampleClassDto> ValidationErrorWithResult()
        {
            var response = new CommandResponse();
            response.AddHandledError("Validation error 1");
            response.AddHandledError("Validation error 2");

            return HttpResponse<SampleClassDto>(response);
        }

        public ActionResult ValidationErrorWithoutResult()
        {
            var response = new CommandResponse();
            response.AddHandledError("Validation error 1");
            response.AddHandledError("Validation error 2");

            return HttpResponse(response);
        }

        public ActionResult<SampleClassDto> FatalErrorWithResult()
        {
            var response = new CommandResponse();
            response.AddUnhandledError("Fatal error 1");

            return HttpResponse<SampleClassDto>(response);
        }

        public ActionResult FatalErrorWithoutResult()
        {
            var response = new CommandResponse();
            response.AddUnhandledError("Fatal error 1");

            return HttpResponse(response);
        }

        public ActionResult OkWithoutResult()
        {
            var response = new CommandResponse();

            return HttpResponse(response);
        } 

        public ActionResult<SampleClassDto> OkWithResultAndMapping()
        {
            var response = new CommandResponse();
            response.Result = new SampleClass();

            return HttpResponse<SampleClassDto>(response);
        }

        public ActionResult OkWithResultAndNoMapping()
        {
            var response = new CommandResponse();
            response.Result = new SomeOtherSampleClass();

            return HttpResponse(response);
        }

        public ActionResult<SampleClassDto> OkWithResulCacheAndMapping()
        {
            var response = new CommandResponse();
            response.Result = new SampleClass();

            return HttpResponse<SampleClassDto>(response, "CACHE_ID");
        }

        public ActionResult OkWithResultCacheAndNoMapping()
        {
            var response = new CommandResponse();
            response.Result = new SomeOtherSampleClass();

            return HttpResponse(response, "CACHE_ID");
        }

        public async Task<ActionResult> OkWithMediatR()
        {
            var response = await Mediator.Send(new SomeCommand());

            return HttpResponse(response);
        }
    }

    public class SomeCommand : IRequest<CommandResponse>
    {

    }

    public class SampleClass: BaseEntity
    {

    }

    public class SampleClassDto: BaseDataTransferObject
    {

    }

    public class SomeOtherSampleClass
    {

    }  
}
