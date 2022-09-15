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
    [IgnoreOnCodeGeneration]
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : BaseController
    {

        /// <summary>
        /// Atualiza as regras de acesso de um usuário
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_USER_ROLES)]
        [HttpPost("update-roles")]
        public async Task<ActionResult<SimpleNamedEntity[]>> UpdateUserRoles(UpdateUserRoles.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<SimpleNamedEntity[]>(result);
        }

        /// <summary>
        /// Adicionar um controle de acesso a um usuário
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.OVERRIDE_USER_CLAIMS)]
        [HttpPost("add-claim")]
        public async Task<ActionResult<ClaimOverride[]>> AddClaimToUser(AddClaimToUser.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<ClaimOverride[]>(result);
        }

        /// <summary>
        /// Adicionar um controle de acesso a um usuário
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.OVERRIDE_USER_CLAIMS)]
        [HttpPost("remove-claim")]
        public async Task<ActionResult<ClaimOverride[]>> RemoveClaimFromUser(RemoveClaimFromUser.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<ClaimOverride[]>(result);
        }

    }
}
