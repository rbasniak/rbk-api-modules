using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Identity.Core.DataTransfer.Claims;
using rbkApiModules.Identity.Core.DataTransfer.Roles;
using rbkApiModules.Identity.Core.DataTransfer.Tenants;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Core;

[Authorize]
[IgnoreOnCodeGeneration]
[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : BaseController
{
    #region claims

    /// <summary>
    /// Lista as permissões de acesso
    /// </summary>
    [HttpGet("claims")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_CLAIMS)]
    public async Task<ActionResult<ClaimDetails[]>> GetAllClaims(CancellationToken cancellation)
    {
        return HttpResponse<ClaimDetails[]>(await Mediator.Send(new GetAllClaims.Request(), cancellation));
    }

    /// <summary>
    /// Criar permissão de acesso
    /// </summary>
    [HttpPost("claims")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_CLAIMS)]
    public async Task<ActionResult<ClaimDetails>> CreateClaim(CreateClaim.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        return HttpResponse<ClaimDetails>(result);
    }

    /// <summary>
    /// Atualiza os detalhes de uma permissão de acessos
    /// </summary>
    [HttpPut("claims")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_CLAIMS)]
    public async Task<ActionResult<ClaimDetails>> UpdateClaim(UpdateClaim.Request data, CancellationToken cancellation)
    {
        return HttpResponse<ClaimDetails>(await Mediator.Send(data, cancellation));
    }

    /// <summary>
    /// Apaga uma permissão de acesso
    /// </summary>
    [HttpDelete("claims/{id}")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_CLAIMS)]
    public async Task<ActionResult> DeleteClaim(Guid id, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(new DeleteClaim.Request { Id = id }, cancellation));
    }

    /// <summary>
    /// Protege um acesso
    /// </summary>
    [HttpPost("claims/protect")]
    [RbkAuthorize(AuthenticationClaims.CHANGE_CLAIM_PROTECTION)]
    public async Task<ActionResult<ClaimDetails>> ProtectClaim(ProtectClaim.Request data, CancellationToken cancellation)
    {
        return HttpResponse<ClaimDetails>(await Mediator.Send(data, cancellation));
    }

    /// <summary>
    /// Desprotege um acesso
    /// </summary>
    [HttpPost("claims/unprotect")]
    [RbkAuthorize(AuthenticationClaims.CHANGE_CLAIM_PROTECTION)]
    public async Task<ActionResult<ClaimDetails>> UnprotectClaim(UnprotectClaim.Request data, CancellationToken cancellation)
    {
        return HttpResponse<ClaimDetails>(await Mediator.Send(data, cancellation));
    }

    #endregion

    #region roles 

    [HttpGet("roles")]
    public async Task<ActionResult<RoleDetails[]>> GetAllRoles(CancellationToken cancellation)
    {
        return HttpResponse<RoleDetails[]>(await Mediator.Send(new GetAllRoles.Request(), cancellation));
    }

    [HttpPost("roles")]
    public async Task<ActionResult<RoleDetails>> CreateRole(CreateRole.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        return HttpResponse<RoleDetails>(result);
    }

    [HttpPut("roles")]
    public async Task<ActionResult<RoleDetails>> UpdateRole(RenameRole.Request data, CancellationToken cancellation)
    {
        return HttpResponse<RoleDetails>(await Mediator.Send(data, cancellation));
    }

    [HttpDelete("roles/{id}")]
    public async Task<ActionResult> DeleteRole(Guid id, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(new DeleteRole.Request { Id = id }, cancellation));
    }

    [HttpPost("roles/update-claims")]
    public async Task<ActionResult<RoleDetails>> UpdateRoleClaims(UpdateRoleClaims.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        return HttpResponse<RoleDetails>(result);
    }

    #endregion

    #region users 

    [HttpGet("users")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_USERS)]
    public async Task<ActionResult<UserDetails[]>> GetAllUsers(CancellationToken cancellation)
    {
        return HttpResponse<UserDetails[]>(await Mediator.Send(new GetAllUsers.Request(), cancellation));
    }

    [RbkAuthorize(AuthenticationClaims.MANAGE_USER_ROLES)]
    [HttpPost("users/set-roles")]
    public async Task<ActionResult<UserDetails>> UpdateUserRoles(ReplaceUserRoles.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        return HttpResponse<UserDetails>(result);
    }

    [RbkAuthorize(AuthenticationClaims.OVERRIDE_USER_CLAIMS)]
    [HttpPost("users/add-claims")]
    public async Task<ActionResult<UserDetails>> AddClaimToUser(AddClaimOverride.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        return HttpResponse<UserDetails>(result);
    }

    [RbkAuthorize(AuthenticationClaims.OVERRIDE_USER_CLAIMS)]
    [HttpPost("users/remove-claims")]
    public async Task<ActionResult<UserDetails>> RemoveClaimFromUser(RemoveClaimOverride.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        return HttpResponse<UserDetails>(result);
    }

    #endregion

    #region tenants 

    [HttpGet("tenants")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_TENANTS)]
    public async Task<ActionResult<TenantDetails[]>> GetAllTenantsAuthenticated(CancellationToken cancellation)
    {
        return HttpResponse<TenantDetails[]>(await Mediator.Send(new GetAllTenants.Request(), cancellation));
    }

    [HttpGet("tenants")]
    [AllowAnonymous]
    public async Task<ActionResult<TenantDetails[]>> GetAllTenantsAnonymous(CancellationToken cancellation)
    {
        var temp = HttpContext.User.Identity.Name;
        return HttpResponse<TenantDetails[]>(await Mediator.Send(new GetAllTenants.Request(), cancellation));
    }

    [HttpPost("tenants")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_TENANTS)]
    public async Task<ActionResult<TenantDetails>> CreateTenant(CreateTenant.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        return HttpResponse<TenantDetails>(result);
    }

    [HttpPut("tenants")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_TENANTS)]
    public async Task<ActionResult<TenantDetails>> UpdateTenant(UpdateTenant.Request data, CancellationToken cancellation)
    {
        return HttpResponse<TenantDetails>(await Mediator.Send(data, cancellation));
    }

    [HttpDelete("tenants/{id}")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_TENANTS)]
    public async Task<ActionResult> DeleteTenant(string id, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(new DeleteTenant.Request { Alias = id }, cancellation));
    }

    #endregion 
}