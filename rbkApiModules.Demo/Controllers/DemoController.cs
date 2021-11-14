using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;

namespace rbkApiModules.Demo
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class DemoController : BaseController
    {
        private static List<Entity> _database = new List<Entity>();
        private static List<Country> _countries = new List<Country>
        {
            new Country() { Id = Guid.NewGuid(), Name = "United States" },
            new Country() { Id = Guid.NewGuid(), Name = "United Kingdom" },
            new Country() { Id = Guid.NewGuid(), Name = "France" },
            new Country() { Id = Guid.NewGuid(), Name = "Canada" },
            new Country() { Id = Guid.NewGuid(), Name = "Brazil" },
        };

        public DemoController()
        {
            if (_database.Count == 0)
            {
                _database.Add(new Entity { Id = Guid.NewGuid(), Name = "Bruce Wayne", Company = "Wayne Enterprises", Country = _countries[1] });
                _database.Add(new Entity { Id = Guid.NewGuid(), Name = "Tony Stark", Company = "Stark Industries", Country = _countries[3] });
                _database.Add(new Entity { Id = Guid.NewGuid(), Name = "Coyote", Company = "ACME Inc", Country = _countries[4] });
            }
        }

        [CodeGenerationScope("project-a", "project-b")]
        [HttpGet]
        public ActionResult All()
        {
            var response = new QueryResponse();
            response.Result = _database;

            return HttpResponse(response);
        }

        [CodeGenerationScope("project-b")]
        [HttpGet("Countries")]
        public ActionResult Countries()
        {
            var response = new QueryResponse();
            response.Result = _countries;

            return HttpResponse(response);
        }


        [HttpPost]
        [CodeGenerationScope("project-a")]
        public ActionResult Create(EntityCreation data)
        {
            var response = new CommandResponse();

            if (data.Name.ToLower() == "basniak")
            {
                response.AddHandledError("Por favor não use o Basniak aqui");

                return HttpResponse(response);
            }

            var element = new Entity
            {
                Id = Guid.NewGuid(),
                Name = data.Name,
                Company = data.Company,
                Country = _countries.First(x => x.Id == data.CountryId)
            };

            _database.Add(element);

            response.Result = element;

            Thread.Sleep(3000);

            return HttpResponse(response);
        }

        [CodeGenerationScope("project-b")]
        [HttpPut]
        public ActionResult Update(EntityUpdate data)
        {
            var response = new CommandResponse();

            if (data.Name.ToLower() == "basniak")
            {
                response.AddHandledError("Por favor não use o Basniak aqui");
                response.AddHandledError("E não esqueça disso");

                return HttpResponse(response);
            }

            var index = _database.FindIndex(x => x.Id == data.Id);
            _database[index] = new Entity
            {
                Id = data.Id,
                Name = data.Name,
                Company = data.Company,
                Country = _countries.First(x => x.Id == data.CountryId)
            };

            response.Result = _database[index];

            Thread.Sleep(3000);

            return HttpResponse(response);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            var response = new CommandResponse();

            _database = _database.Where(x => x.Id != id).ToList();

            Thread.Sleep(3000);

            return HttpResponse(response);
        }
    }

    public class Entity
    {
        public Guid Id { get; set; }

        [Required, MinLength(3), MaxLength(10)]
        [DialogData(OperationType.CreateAndUpdate, "Nome")]
        public string Name { get; set; }

        [Required, MinLength(3), MaxLength(10)]
        [DialogData(OperationType.CreateAndUpdate, "Empresa")]
        public string Company { get; set; }

        [Required]
        [DialogData(OperationType.CreateAndUpdate, "País", Source = DataSource.Store, SourceName = "countries")]
        public Country Country { get; set; }
    }

    public class EntityCreation
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public Guid CountryId { get; set; }
    }

    public class EntityUpdate
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public Guid CountryId { get; set; }
    }

    public class Country : BaseEntity
    {
        public string Name { get; set; }
    }

}
