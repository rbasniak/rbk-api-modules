using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace rbkApiModules.Commons.Core;

/// <summary>
/// Base response class for all <see cref="IDispatcher"/> responses.
/// </summary>
public abstract class BaseResponse
{
    private ProblemDetails _error = default!;
    private object _data = default!;

    public bool IsValid { get; protected set; }

    public ProblemDetails Error
    {
        get
        {
            if (IsValid)
            {
                throw new InvalidOperationException("Cannot access Error when response is valid.");
            }

            return _error;
        }
        protected set
        {
            _error = value;
        }
    }

    public object Data
    {
        get
        {
            if (!IsValid)
            {
                throw new InvalidOperationException("Cannot access Data when response is not valid.");
            }

            return _data;
        }
        protected set
        {
            _data = value;
        }
    }
}


/// <summary>
/// Response class for command type requests.
/// </summary>
public sealed class CommandResponse : BaseResponse
{
    internal CommandResponse()
    {
    }

    public static CommandResponse Success()
    {
        return new CommandResponse()
        {
            IsValid = true
        };
    }

    public static CommandResponse Success(object result)
    {
        return new CommandResponse()
        {
            Error = null,
            IsValid = true,
            Data = result
        };
    }

    internal static CommandResponse Failure(ProblemDetails problem)
    {
        return new CommandResponse()
        {
            Error = problem,
            IsValid = false,
            Data = null,
        };
    }

    public static CommandResponse Failure(string message)
    {
        return new CommandResponse()
        {
            Error = new ProblemDetails
            {
                Title = "Command Failure",
                Detail = message,
                Status = StatusCodes.Status400BadRequest
            },
            IsValid = false,
        };
    }

    public static CommandResponse Forbidden(string detail)
    {
        return new CommandResponse()
        {
            Error = new ProblemDetails
            {
                Title = "Forbidden",
                Detail = detail,
                Status = StatusCodes.Status403Forbidden
            },
            IsValid = false,
        };
    }
}

/// <summary>
/// Response class for typed command type requests.
/// </summary>
public sealed class CommandResponse<T> : BaseResponse
{
    private T _data = default!;

    internal CommandResponse()
    {
    }

    public static CommandResponse<T> Success(T result)
    {
        return new CommandResponse<T>()
        {
            Error = null,
            IsValid = true,
            Data = result
        };
    }

    internal static CommandResponse<T> Failure(ProblemDetails problem)
    {
        return new CommandResponse<T>()
        {
            Error = problem,
            IsValid = false,
            Data = default!
        };
    }

    public static CommandResponse<T> Failure(string message)
    {
        return new CommandResponse<T>()
        {
            Error = new ProblemDetails
            {
                Title = "Command Failure",
                Detail = message,
            },
            IsValid = false,
        };
    }

    public new T Data
    {
        get
        {
            if (!IsValid)
            {
                throw new InvalidOperationException("Cannot access Data when response is not valid.");
            }

            return _data;
        }
        private set
        {
            _data = value;
        }
    }
}


/// <summary>
/// Response class for query type requests.
/// </summary>
public sealed class QueryResponse : BaseResponse
{
    internal QueryResponse()
    {
    }

    public static QueryResponse Success()
    {
        return new QueryResponse
        {
            IsValid = true
        };
    }

    public static QueryResponse Success(object result)
    {
        return new QueryResponse()
        {
            Error = null,
            IsValid = true,
            Data = result
        };
    }

    internal static QueryResponse Failure(ProblemDetails problem)
    {
        return new QueryResponse()
        {
            Error = problem,
            IsValid = false,
            Data = null
        };
    }

    public static QueryResponse Failure(string message)
    {
        return new QueryResponse()
        {
            Error = new ProblemDetails
            {
                Title = "Query Failure",
                Detail = message,
            },
            IsValid = false,
        };
    }
}

/// <summary>
/// Response class for typed query type requests.
/// </summary>
public sealed class QueryResponse<T> : BaseResponse
{
    private T _data = default!;

    internal QueryResponse()
    {
    }

    public static QueryResponse<T> Success(T result)
    {
        return new QueryResponse<T>()
        {
            Error = null,
            IsValid = true,
            Data = result
        };
    }

    internal static QueryResponse<T> Failure(ProblemDetails problem)
    {
        return new QueryResponse<T>()
        {
            Error = problem,
            IsValid = false,
            Data = default!
        };
    }

    public static QueryResponse<T> Failure(string message)
    {
        return new QueryResponse<T>()
        {
            Error = new ProblemDetails
            {
                Title = "Query Failure",
                Detail = message,
            },
            IsValid = false,
        };
    }

    public new T Data
    {
        get
        {
            if (!IsValid)
            {
                throw new InvalidOperationException("Cannot access Data when response is not valid.");
            }

            return _data;
        }
        private set
        {
            _data = value;
        }
    }

}
