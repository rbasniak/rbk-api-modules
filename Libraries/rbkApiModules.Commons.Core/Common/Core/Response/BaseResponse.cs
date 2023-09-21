namespace rbkApiModules.Commons.Core;

public class BaseResponse
{
    private readonly HashSet<ErrorMessage> _messages = new HashSet<ErrorMessage>();

    public BaseResponse()
    {
    }

    public BaseResponse(object result) : this()
    {
        Result = result;
    }

    public bool IsValid => Status == CommandStatus.Valid;

    public IEnumerable<ErrorMessage> Errors => _messages?.ToList();

    public object Result { get; set; }

    public CommandStatus Status { get; set; }

    public void AddUnhandledError(Exception exception, string message)
    {
        Status = CommandStatus.HasUnhandledError;

        AddError(exception, ValidationErrorCodes.UNHANDLED, message);
    }

    public void AddUnhandledError(Exception exception)
    {
        Status = CommandStatus.HasUnhandledError;

        // TODO: depending on the scenario, might not be interesting to expose this to the client
        AddError(exception, ValidationErrorCodes.UNHANDLED, exception.ToBetterString());
    }

    public void AddHandledError(string code, string message)
    {
        if (Status != CommandStatus.HasUnhandledError)
        {
            Status = CommandStatus.HasHandledError;
        }

        AddError(exception: null, code, message);
    }

    public void AddHandledError(Exception exception, string message)
    {
        if (Status != CommandStatus.HasUnhandledError)
        {
            Status = CommandStatus.HasHandledError;
        }

        AddError(exception, ValidationErrorCodes.BAD_REQUEST, message);
    }

    private void AddError(Exception exception, string code, string message)
    {
        _messages.Add(new ErrorMessage(exception, code, message));
    }
}