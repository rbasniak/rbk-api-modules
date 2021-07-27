//using FluentValidation;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using rbkApiModules.Infrastructure.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using rbkApiModules.Infrastructure.MediatR.Core;
//using rbkApiModules.Utilities;
//using rbkApiModules.Utilities.Charts.ChartJs;
//using rbkApiModules.Utilities.Charts;
//using rbkApiModules.Diagnostics.Commons;

//namespace rbkApiModules.Analytics.Core
//{
//    public class NormalizePathsAndActions
//    {
//        public class Command : IRequest<CommandResponse>
//        {
//        }

//        public class Handler : BaseCommandHandler<Command>
//        {
//            private readonly IAnalyticModuleStore _context;

//            public Handler(IHttpContextAccessor httpContextAccessor, IAnalyticModuleStore context)
//                : base(httpContextAccessor)
//            {
//                _context = context;
//            }

//            protected override async Task<object> ExecuteAsync(Command request)
//            {
//                var results = await _context.GetStatisticsAsync();

//                foreach (var item in results)
//                {
//                    if (!item.Action.ToLower().StartsWith("get /") && item.Action.ToLower().StartsWith("get "))
//                    {
//                        item.Action = item.Action.Replace("GET ", "GET /");
//                    }

//                    if (!item.Action.ToLower().StartsWith("post /") && item.Action.ToLower().StartsWith("post "))
//                    {
//                        item.Action = item.Action.Replace("POST ", "POST /");
//                    }

//                    if (!item.Action.ToLower().StartsWith("put /") && item.Action.ToLower().StartsWith("put "))
//                    {
//                        item.Action = item.Action.Replace("PUT ", "PUT /");
//                    }

//                    if (!item.Action.ToLower().StartsWith("delete /") && item.Action.ToLower().StartsWith("delete "))
//                    {
//                        item.Action = item.Action.Replace("DELETE ", "DELETE /");
//                    }

//                    if (!item.Path.ToLower().StartsWith("get /") && item.Path.ToLower().StartsWith("get "))
//                    {
//                        item.Path = item.Path.Replace("GET ", "GET /");
//                    }

//                    if (!item.Path.ToLower().StartsWith("post /") && item.Path.ToLower().StartsWith("post "))
//                    {
//                        item.Path = item.Path.Replace("POST ", "POST /");
//                    }

//                    if (!item.Path.ToLower().StartsWith("put /") && item.Path.ToLower().StartsWith("put "))
//                    {
//                        item.Path = item.Path.Replace("PUT ", "PUT /");
//                    }

//                    if (!item.Path.ToLower().StartsWith("delete /") && item.Path.ToLower().StartsWith("delete "))
//                    {
//                        item.Path = item.Path.Replace("DELETE ", "DELETE /");
//                    }
//                }

//                _context.save

//                return await Task.FromResult((object)null);
//            }
//        }
//    }
//}
