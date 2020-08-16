using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Authentication;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace AspNetCoreApiTemplate.Api
{
    /// <summary>
    /// Controller para acessar as funcionalidades de regras de acesso
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : BaseController
    {
        /// <summary>
        /// Lista os regra de acessos 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Roles.Details[]>> All()
        {
            return HttpResponse<Roles.Details[]>(await Mediator.Send(new GetAllRoles.Command()));
        }

        /// <summary>
        /// Lista os detalhes de uma regra de acessos 
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Roles.Details>> Details(Guid id)
        {
            return HttpResponse<Roles.Details>(await Mediator.Send(new GetRoleDetails.Command { Id = id }));
        }

        /// <summary>
        /// Cria uma nova regra de acesso
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SimpleNamedEntity>> Create(CreateRole.Command data)
        {
            return HttpResponse<SimpleNamedEntity>(await Mediator.Send(data));
        }

        /// <summary>
        /// Apaga uma regra de acesso
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return HttpResponse(await Mediator.Send(new DeleteRole.Command { RoleId = id }));
        }

        /// <summary>
        /// Adiciona um controle de acesso a um regra de acesso
        /// </summary>
        [HttpPost("add")]
        public async Task<ActionResult> AddRole(AddClaimToRole.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        /// <summary>
        /// Adiciona um controle de acesso de uma regra de acesso
        /// </summary>
        [HttpPost("remove")]
        public async Task<ActionResult> RemoveRole(RemoveClaimFromRole.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        } 
    }
}
