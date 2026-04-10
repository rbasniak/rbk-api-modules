namespace rbkApiModules.Commons.Core;

public class InternalException : Exception
{
    protected InternalException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
    protected InternalException(string message, Exception innerException, int statusCode) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
    public int StatusCode { get; }
}


public class ExpectedInternalException : InternalException
{
    public ExpectedInternalException(string message) : base(message, 400)
    {
    }
    public ExpectedInternalException(string message, Exception innerException) 
        : base(message, innerException, 400)
    {
    }
}

public class UnexpectedInternalException : InternalException
{
    public UnexpectedInternalException(string message) : base(message, 500)
    {
    }

    public UnexpectedInternalException(string message, Exception innerException) 
        : base(message, innerException, 500)
    {
    }
}
