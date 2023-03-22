namespace rbkApiModules.Identity.Core;

public class CustomValidationResult
{
    private CustomValidationResult()
    {
        Errors = new Enum[0];
    }

    public Enum[] Errors { get; private set; }

    public bool IsSuccess => Errors.Length == 0;
    public bool HasErrors => Errors.Length > 0;

    public static CustomValidationResult Success()
    {
        return new CustomValidationResult();
    }

    public static CustomValidationResult Failure(params Enum[] errors)
    {
        if (errors == null || errors.Length == 0) throw new ArgumentNullException(nameof(errors)); 

        return new CustomValidationResult
        {
            Errors = errors,
        };
    }
}