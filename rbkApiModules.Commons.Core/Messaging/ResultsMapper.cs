using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace rbkApiModules.Commons.Core;

public static class ResultsMapper
{
    public static IResult FromResponse(BaseResponse response)
    {
        if (response.IsValid)
        {
            if (response.Data != null)
            {
                return Results.Ok(response.Data);
            }

            return Results.Ok();
        }
        else
        {
            if (response.Error == null)
            {
                return Results.Problem("An unexpected error occurred: response was not success but was missing the error details");
            }

            if (response.Error is ValidationProblemDetails validationProblem)
            {
                validationProblem.Status ??= StatusCodes.Status400BadRequest;

                return Results.ValidationProblem(
                    validationProblem.Errors,
                    title: validationProblem.Title,
                    type: validationProblem.Type,
                    statusCode: StatusCodes.Status400BadRequest,
                    detail: validationProblem.Detail,
                    instance: validationProblem.Instance);
            }

            var status = response.Error.Status ?? StatusCodes.Status500InternalServerError;
            return Results.Problem(
                detail: response.Error.Detail,
                statusCode: status,
                title: response.Error.Title,
                type: response.Error.Type,
                instance: response.Error.Instance);
        }
    }
}