using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Authentication.AuthenticationGroups;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Controller para acesso das funcionalidades de autorização e controle de acesso
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
        /// Criar regra de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpPost]
        public async Task<ActionResult<Roles.Details>> Create(CreateRole.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<Roles.Details>(result);
        }

        /// <summary>
        /// Atualiza os detalhes de uma regra de acessos 
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpPut]
        public async Task<ActionResult<Roles.Details>> Update(UpdateRole.Command data)
        {
            return HttpResponse<Roles.Details>(await Mediator.Send(data));
        }

        /// <summary>
        /// Apaga uma regra de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return HttpResponse(await Mediator.Send(new DeleteRole.Command { RoleId = id }));
        }

        /// <summary>
        /// Adicionar um controle de acesso a uma regra de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpPost("update-claims")]
        public async Task<ActionResult<Roles.Details>> UpdateRoleClaims(UpdateRoleClaims.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<Roles.Details>(result);
        }

    }
}
