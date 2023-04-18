﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core.CodeGeneration;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Commons.Core.UiDefinitions;

[IgnoreOnCodeGeneration]
[Route("api/ui-definitions")]
[ApiController]
public class UiDefinitionsController : BaseController
{

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<object> All([FromServices] UIDefinitionOptions options)
    {
        try
        {
            var result = new Dictionary<string, FormDefinition>();

            var assemblies = options.Assemblies;

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

            var temp = new JsonResult(result, new System.Text.Json.JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            return temp;
        } 
        catch (Exception ex) 
        {
            return StatusCode(500, new[] { ex.ToBetterString() });
        } 
    }
} 

