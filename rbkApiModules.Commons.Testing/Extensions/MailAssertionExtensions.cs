using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using rbkApiModules.Commons.Core.Authentication;
using rbkApiModules.Commons.Testing;
using Shouldly;

public static class MailAssertionExtensions
{
    public static string GetMailOutputFolder<TProgram>(this RbkTestingServer<TProgram> server) where TProgram : class
    {
        var emailOptions = server.Server.Services.GetRequiredService<IOptions<AuthEmailOptions>>();
        var folder = emailOptions.Value.TestMode.OutputFolder;

        return folder;
    }

    public static void ClearMailsFolder<TProgram>(this RbkTestingServer<TProgram> server) where TProgram : class
    {
        var folder = server.GetMailOutputFolder();

        if (!Directory.Exists(folder))
        {
            throw new DirectoryNotFoundException(folder);
        }

        var files = Directory.GetFiles(folder);

        foreach (var file in files)
        {
            File.Delete(file);
        }
    }

    public static void ShouldHaveSentEmail<TProgram>(this RbkTestingServer<TProgram> server, Action<MailAssertionOptions> action) where TProgram : class
    {
        var options = new MailAssertionOptions();

        action(options);

        var folder = server.GetMailOutputFolder();

        var files = Directory.GetFiles(folder, "*.eml");

        
        files.Count().ShouldBe(1);

        var mailContents = File.ReadAllLines(files[0]);

        var message = MimeMessage.Load(files[0]);

        message.GetRecipients().Count().ShouldBe(1);
        message.GetRecipients()[0].Address.ShouldBe(options.Receiver);

        foreach (var text in options.ThingsToCheckOnTitle)
        {
            message.Subject.Contains(text).ShouldBeTrue($"E-mail title doesn't contain '{text}'");
        }

        server.ClearMailsFolder();
    }
}

public class MailAssertionOptions
{
    public List<string> ThingsToCheckOnTitle { get; private set; } = [];
    public string Receiver { get; private set; } = string.Empty!;

    public MailAssertionOptions()
    {
        ThingsToCheckOnTitle = new List<string>();
    }

    public MailAssertionOptions ToAddress(string receiver)
    {
        Receiver = receiver;

        return this;
    }

    public MailAssertionOptions WithTileContaining(string text)
    {
        ThingsToCheckOnTitle.Add(text);

        return this;
    }
}