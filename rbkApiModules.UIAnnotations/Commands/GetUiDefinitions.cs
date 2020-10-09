using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.UIAnnotations
{
    public class GetUiDefinitions
    {
        public class Command : IRequest<QueryResponse>
        {
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
            }
        }

        public class Handler : BaseQueryHandler<Command, DbContext>
        {
            private readonly UIDefinitionOptions _options;
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, UIDefinitionOptions options)
                : base(context, httpContextAccessor)
            {
                _options = options;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var result = new Dictionary<string, FormDefinition>();

                var assemblies = _options.Assemblies;

                var builder = new DialogDataBuilderService();

                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        var createInputs = builder.Build(type, OperationType.Create);
                        var updateInputs = builder.Build(type, OperationType.Update);

                        if (typeof(BaseEntity).IsAssignableFrom(type) && updateInputs.Any())
                        {
                            updateInputs.Insert(0, new FormGroup 
                            {
                                Controls = new List<InputControl>()
                                {
                                    new InputControl("id", typeof(string), new RequiredAttribute(), null, null, 
                                        new DialogDataAttribute(OperationType.Update, "Id") 
                                        { 
                                            IsVisible = false,
                                        })
                                }
                            });
                        }

                        if (createInputs.Count > 0 || updateInputs.Count > 0)
                        {
                            result.Add(Char.ToLower(type.Name[0]) + type.Name.Substring(1), new FormDefinition(createInputs, updateInputs));
                        }
                    }
                }

                return await Task.FromResult(result);
            }
        }
    }
}
