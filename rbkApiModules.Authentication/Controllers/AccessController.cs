using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Authentication.AuthenticationGroups;
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
    public class AccessController : BaseController
    {
        /// <summary>
        /// Lista as permissões de acesso
        /// </summary>
        [HttpGet("claims")]
        public async Task<ActionResult<ClaimDetails[]>> AllClaims()
        {
            return HttpResponse<ClaimDetails[]>(await Mediator.Send(new GetAllClaims.Command()));
        }

        /// <summary>
        /// Lista os regra de acessos 
        /// </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<Roles.Details[]>> AllRoles()
        {
            return HttpResponse<Roles.Details[]>(await Mediator.Send(new GetAllRoles.Command()));
        }

        /// <summary>
        /// Criar regra de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpPost("roles")]
        public async Task<ActionResult<SimpleNamedEntity>> CreateRole(CreateRole.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<SimpleNamedEntity>(result);
        }

        /// <summary>
        /// Atualiza os detalhes de uma regra de acessos 
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpPut("roles")]
        public async Task<ActionResult<Roles.Details>> UpdateRole(UpdateRole.Command data)
        {
            return HttpResponse<Roles.Details>(await Mediator.Send(data));
        }

        /// <summary>
        /// Apaga uma regra de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpDelete("roles/{id}")]
        public async Task<ActionResult> DeleteRole(Guid id)
        {
            return HttpResponse(await Mediator.Send(new DeleteRole.Command { RoleId = id }));
        }

        /// <summary>
        /// Adicionar um controle de acesso a uma regra de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpPost("role/add-claim")]
        public async Task<ActionResult> AddClaimToRole(AddClaimToRole.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        /// <summary>
        /// Remover um controle de acesso de uma regra de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_ROLES)]
        [HttpPost("role/remove-claim")]
        public async Task<ActionResult> RemoveClaimFromRole(RemoveClaimFromRole.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        /// <summary>
        /// Adicionar uma regra de acesso a um usuário
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_USER_ROLES)]
        [HttpPost("user/add-role")]
        public async Task<ActionResult> AddRoleToUser(AddRoleToUser.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        /// <summary>
        /// Remover uma regra de acesso de um usuário
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_USER_ROLES)]
        [HttpPost("user/remove-role")]
        public async Task<ActionResult> RemoveRoleFromUser(RemoveRoleFromUser.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        /// <summary>
        /// Adicionar um controle de acesso a um usuário
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.OVERRIDE_USER_CLAIMS)]
        [HttpPost("user/add-claim")]
        public async Task<ActionResult> AddClaimToUser(AddClaimToUser.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        /// <summary>
        /// Adicionar um controle de acesso a um usuário
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.OVERRIDE_USER_CLAIMS)]
        [HttpPost("user/remove-claim")]
        public async Task<ActionResult> RemoveClaimFromUser(RemoveClaimFromUser.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }


    }
}
