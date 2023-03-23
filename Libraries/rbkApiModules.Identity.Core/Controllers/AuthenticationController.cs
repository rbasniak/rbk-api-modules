using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;
using Microsoft.Win32;

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
    [HttpPost("login")]
    public async Task<ActionResult<JwtResponse>> LoginWithNegotiate(UserLogin.Request data, CancellationToken cancellation)
    {
        var isAuthenticated = HttpContext.User.Identity.IsAuthenticated;

        data.Username = HttpContext.User.Identity.Name.Split('\\').Last().ToLower();
        data.AuthenticationMode = AuthenticationMode.Windows;

        try
        {
            return HttpResponse<JwtResponse>(await Mediator.Send(data, cancellation));
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
    [HttpPost("login")]
    public async Task<ActionResult<JwtResponse>> LoginWithCredentials(UserLogin.Request data, CancellationToken cancellation)
    {
        data.AuthenticationMode = AuthenticationMode.Credentials;

        try
        {
            return HttpResponse<JwtResponse>(await Mediator.Send(data, cancellation));
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
    [HttpPost("refresh-token")]
    public async Task<ActionResult<JwtResponse>> RefreshToken(RenewAccessToken.Request data, CancellationToken cancellation)
    {
        return HttpResponse<JwtResponse>(await Mediator.Send(data, cancellation));
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
    [HttpPost("switch-domain")]
    public async Task<ActionResult<JwtResponse>> SwitchDomain(SwitchDomain.Request data, CancellationToken cancellation)
    {
        return HttpResponse<JwtResponse>(await Mediator.Send(data, cancellation));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePassword.Request data, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(data, cancellation));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<JwtResponse>> RegisterAnonymously(Register.Request data, CancellationToken cancellation)
    {
        return HttpResponse<JwtResponse>(await Mediator.Send(data, cancellation));
    }

    [RbkAuthorize(AuthenticationClaims.MANAGE_USERS)]
    [HttpPost("create-user")]
    public async Task<ActionResult<JwtResponse>> CreateUser(CreateUser.Request data, CancellationToken cancellation)
    {
        return HttpResponse<JwtResponse>(await Mediator.Send(data, cancellation));
    }
}