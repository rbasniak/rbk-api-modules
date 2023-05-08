namespace rbkApiModules.Commons.Core;

public class AuthEmailOptions
{
    public ServerOptions Server { get; set; }
    public SenderOptions Sender { get; set; }
    public EmailContentOptions EmailData { get; set; }
    public TestOptions TestMode { get; set; }
}

public class TestOptions
{
    public bool Enabled { get; set; }
    public string OutputFolder { get; set; }
}

public class ServerOptions
{
    public string SmtpHost { get; set; }
    public int Port { get; set; }
    public bool SSL { get; set; }
}

public class SenderOptions
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class EmailContentOptions
{
    public string MainColor { get; set; }
    public string FontColor { get; set; }
    public string Logo { get; set; }
    public string SuportEmail { get; set; }
    public string AccountDetailsUrl { get; set; }
    public string PasswordResetUrl { get; set; }
    public string ConfirmationSuccessUrl { get; set; }
    public string ConfirmationFailedUrl { get; set; }
}