﻿using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
    [IgnoreOnCodeGeneration]
    [HttpPost("login")]
    public async Task<ActionResult<JwtResponse>> Login(UserLogin.Command data, CancellationToken cancellation)
    {
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
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            };
        }
        catch (Exception ex)
        {
            return new ContentResult()
            {
                Content = JsonSerializer.Serialize(new[] { ex.Message }),
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }
    }

    [AllowAnonymous]
    [IgnoreOnCodeGeneration]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<JwtResponse>> RefreshToken(RenewAccessToken.Command data, CancellationToken cancellation)
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
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        };
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("reset-password")]
    public async Task<ActionResult> SendResetPasswordEmail(RequestPasswordReset.Command data, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(data, cancellation));
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("redefine-password")]
    public async Task<ActionResult> RedefinePassword(RedefinePassword.Command data, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(data, cancellation));
    }

    [AllowAnonymous]
    [HttpPost("resend-confirmation")]
    [RbkAuthorize(AuthenticationClaims.MANAGE_USERS)]
    public async Task<ActionResult> ResendEmailConfirmation(ResendEmailConfirmation.Command data, CancellationToken cancellation)
    {
        return HttpResponse(await Mediator.Send(data, cancellation));
    }

    [AllowAnonymous]
    [IgnoreOnCodeGeneration]
    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(string email, string code, string tenant, CancellationToken cancellation)
    {
        var response = await Mediator.Send(new ConfirmUserEmail.Command() { Email = email, ActivationCode = code, Tenant = tenant }, cancellation);

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
}