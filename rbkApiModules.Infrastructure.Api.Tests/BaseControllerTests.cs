//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using Shouldly;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Xunit;

//namespace rbkApiModules.Infrastructure.Api.Tests
//{
//    public class BaseControllerTests
//    {
//        private TestController()
//        [Fact]
//        public void Is_Mediatr_Injection_Working()
//        {

//        }

//        [Fact]
//        public void Has_Authorize_Attribute()
//        {
//            var attributes = (IList<AuthorizeAttribute>)typeof(BaseController).GetCustomAttributes(typeof(AuthorizeAttribute), false);

//            attributes.Count.ShouldBe(1);
//        }
//    }

//    public class TestController: BaseController
//    {

//    }
//}
