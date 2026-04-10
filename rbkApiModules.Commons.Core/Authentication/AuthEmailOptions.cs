namespace rbkApiModules.Commons.Core.Authentication;

public class AuthEmailOptions
{
    public ServerOptions Server { get; set; } = new();
    public SenderOptions Sender { get; set; } = new();
    public EmailContentOptions EmailData { get; set; } = new();
    public TestOptions TestMode { get; set; } = new();
}

public class TestOptions
{
    public bool Enabled { get; set; }
    public string OutputFolder { get; set; } = string.Empty;
}

public class ServerOptions
{
    public string SmtpHost { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool SSL { get; set; }
}

public class SenderOptions
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class EmailContentOptions
{
    public string MainColor { get; set; } = string.Empty;
    public string FontColor { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public string SuportEmail { get; set; } = string.Empty;
    public string AccountDetailsUrl { get; set; } = string.Empty;
    public string PasswordResetUrl { get; set; } = string.Empty;
    public string ConfirmationSuccessUrl { get; set; } = string.Empty;
    public string ConfirmationFailedUrl { get; set; } = string.Empty;
}