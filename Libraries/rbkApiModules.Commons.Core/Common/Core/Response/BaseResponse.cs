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

    public void AddUnhandledError(string message)
    {
        Status = CommandStatus.HasUnhandledError;

        AddError(ValidationErrorCodes.UNHANDLED, message);
    }

    public void AddUnhandledError(Exception exception)
    {
        Status = CommandStatus.HasUnhandledError;

        // TODO: depending on the scenario, might not be interesting to expose this to the client
        AddError(ValidationErrorCodes.UNHANDLED, exception.ToBetterString());
    }

    public void AddHandledError(string code, string message)
    {
        if (Status != CommandStatus.HasUnhandledError)
        {
            Status = CommandStatus.HasHandledError;
        }

        AddError(code, message);
    }

    public void AddHandledError(string message)
    {
        AddError(ValidationErrorCodes.BAD_REQUEST, message);
    }

    private void AddError(string code, string message)
    {
        _messages.Add(new ErrorMessage(code, message));
    }
}