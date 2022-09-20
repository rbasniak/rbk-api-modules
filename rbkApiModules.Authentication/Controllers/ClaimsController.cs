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
    public class ClaimsController : BaseController
    {
        /// <summary>
        /// Lista as permissões de acesso
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ClaimDetails[]>> All()
        {
            return HttpResponse<ClaimDetails[]>(await Mediator.Send(new GetAllClaims.Command()));
        }

        /// <summary>
        /// Criar permissão de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_CLAIMS)]
        [HttpPost]
        public async Task<ActionResult<ClaimDetails>> Create(CreateClaim.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<ClaimDetails>(result);
        }

        /// <summary>
        /// Atualiza os detalhes de uma permissão de acessos
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_CLAIMS)]
        [HttpPut]
        public async Task<ActionResult<ClaimDetails>> Update(UpdateClaim.Command data)
        {
            return HttpResponse<ClaimDetails>(await Mediator.Send(data));
        }

        /// <summary>
        /// Protege um acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.CAN_OVERRIDE_CLAIM_PROTECTION)]
        [HttpPost("protect")]
        public async Task<ActionResult<ClaimDetails>> Protect(ProtectClaim.Command data)
        {
            return HttpResponse<ClaimDetails>(await Mediator.Send(data));
        }

        /// <summary>
        /// Desprotege um acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.CAN_OVERRIDE_CLAIM_PROTECTION)]
        [HttpPost("unprotect")]
        public async Task<ActionResult<ClaimDetails>> Unprotect(UnprotectClaim.Command data)
        {
            return HttpResponse<ClaimDetails>(await Mediator.Send(data));
        }

        /// <summary>
        /// Apaga uma permissão de acesso
        /// </summary>
        [RbkAuthorize(Claim = AuthenticationClaims.MANAGE_CLAIMS)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            return HttpResponse(await Mediator.Send(new DeleteClaim.Command { ClaimId = id }));
        }

    }
}
