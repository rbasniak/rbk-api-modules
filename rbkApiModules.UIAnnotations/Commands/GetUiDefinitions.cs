using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR;
using System;
using System.Collections.Generic;
using System.Reflection;
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
