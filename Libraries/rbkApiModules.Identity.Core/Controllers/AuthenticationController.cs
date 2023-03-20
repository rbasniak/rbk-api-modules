using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;

namespace rbkApiModules.Identity.Core;

[IgnoreOnCodeGeneration]
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : BaseController
{
    private readonly AuthEmailOptions _authEmailOptions;

    public AuthenticationController(IOptions<AuthEmailOptions> config) : base()
    {
        _authEmailOptions = config.Value;
    }

    [Authorize]
    [IgnoreOnCodeGeneration]
    [HttpPost("login")]
    public async Task<ActionResult<JwtResponse>> LoginWithNegotiate(UserLogin.Request data, CancellationToken cancellation)
    {
        var isAuthenticated = HttpContext.User.Identity.IsAuthenticated;

        data.Username = HttpContext.User.Identity.Name.Split('\\').Last().ToLower();
        data.AuthenticationMode = AuthenticationMode.Windows;

        try
        {
            var result = await Mediator.Send(data, cancellation);

            if (result.IsValid)
            {
                return HttpResponse<JwtResponse>(result);
            }
            else
            {
                return new ContentResult()
                {
                    Content = JsonSerializer.Serialize(result.Errors.Select(x => x.Message)),
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            };
        }
        catch (Exception ex)
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(new[] { ex.Message }),
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }

    [AllowAnonymous]
    [IgnoreOnCodeGeneration]
    [HttpPost("login")]
    public async Task<ActionResult<JwtResponse>> LoginWithCredentials(UserLogin.Request data, CancellationToken cancellation)
    {
        data.AuthenticationMode = AuthenticationMode.Credentials;

        try
        {
            var result = await Mediator.Send(data, cancellation);

            if (result.IsValid)
            {
                return HttpResponse<JwtResponse>(result);
            }
            else
            {
                return new ContentResult()
                {
                    Content = JsonSerializer.Serialize(result.Errors.Select(x => x.Message)),
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            };
        }
        catch (Exception ex)
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(new[] { ex.Message }),
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }

    [AllowAnonymous]
    [IgnoreOnCodeGeneration]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<JwtResponse>> RefreshToken(RenewAccessToken.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        if (result.IsValid)
        {
            return HttpResponse<JwtResponse>(result);
        }
        else
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(result.Errors.Select(x => x.Message)),
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        };
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("reset-password")]
    public async Task<ActionResult> SendResetPasswordEmail(RequestPasswordReset.Request data, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(data, cancellation));
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("redefine-password")]
    public async Task<ActionResult> RedefinePassword(RedefinePassword.Request data, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(data, cancellation));
    }

    [AllowAnonymous]
    [HttpPost("resend-confirmation")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_USERS)]
    public async Task<ActionResult> ResendEmailConfirmation(ResendEmailConfirmation.Request data, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(data, cancellation));
    }

    [AllowAnonymous]
    [IgnoreOnCodeGeneration]
    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(string email, string code, string tenant, CancellationToken cancellation)
    {
        var response = await Mediator.Send(new ConfirmUserEmail.Request() { Email = email, ActivationCode = code, Tenant = tenant }, cancellation);

        if (response.IsValid)
        {
            return Redirect(_authEmailOptions.EmailData.ConfirmationSuccessUrl);
        }
        else
        {
            // TODO: log here
            return Redirect(_authEmailOptions.EmailData.ConfirmationFailedUrl);
        }
    }

    [Authorize]
    [IgnoreOnCodeGeneration]
    [HttpPost("switch-domain")]
    public async Task<ActionResult<JwtResponse>> SwitchDomain(SwitchDomain.Request data, CancellationToken cancellation)
    {
        var result = await Mediator.Send(data, cancellation);

        if (result.IsValid)
        {
            return HttpResponse<JwtResponse>(result);
        }
        else
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(result.Errors.Select(x => x.Message)),
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        };
    }
}