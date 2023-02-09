using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Testing.Core;

public static class MailAssertionExtensions
{
    public static string GetMailOutputFolder(this BaseServerFixture fixture) 
    {
        var emailOptions = fixture.Server.Services.GetService<IOptions<AuthEmailOptions>>();
        var folder = emailOptions.Value.TestMode.OutputFolder;

        return folder;
    }

    public static void ClearMailsFolder(this BaseServerFixture fixture) 
    {
        var folder = fixture.GetMailOutputFolder();

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

    public static void ShouldHaveSentEmail(this BaseServerFixture fixture, Action<MailAssertionOptions> action)
    {
        var options = new MailAssertionOptions();

        action(options);

        var folder = fixture.GetMailOutputFolder();

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

        fixture.ClearMailsFolder();
    }
}