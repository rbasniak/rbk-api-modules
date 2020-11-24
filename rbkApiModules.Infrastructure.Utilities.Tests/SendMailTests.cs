using rbkApiModules.Infrastructure.Utilities.Email;
using rbkApiModules.Infrastructure.Utilities.Email.Models;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace rbkApiModules.Infrastructure.Utilities.Tests
{
    public class SendMailTests : IDisposable
    {
        private static readonly string StarImageBase64 = "iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeBAMAAADJH" + 
            "rORAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAtUExURf///8/Pz/v7+zAwMCgoKPf3939/" + 
            "fwAAAHR0dAQEBMfHx2xsbCAgIPPz87+/v39ufroAAAAJcEhZcwAADsMAAA7DAcdvqGQAAACjSURBVCj" + 
            "PdY8xCsMwDEU/BpusHXOPgI+QJUPpWQohEEKm3qG5QTN57DGy9D6VY0tI0GrIR/Del4M/c7Gr665mD+" + 
            "Nq9nbaDB7fdy2E+dNroV3w2rgpT0Jz5grXU8wHfE0BiwY00uxioq+X5pBxJZx4Fg6FCyZixVhkjC8xz" + 
            "oLgRXCR3kkgZ6j/wcmg0nLzTa6U5odcKcJT4ySMyex+MDiw49cAX5tWPIf28r2yAAAAAElFTkSuQmCC";

        private static readonly string BasicHtml = "<!DOCTYPE html>\r\n<html>\r\n<body>\r\n<h1>Title</h1>\r\n<p>Paragraph.</p>\r\n</body>\r\n</html>";
        private static readonly string InlineHtml = "<!DOCTYPE html>\r\n<html>\r\n<body>\r\n<h1>Title</h1>\r\n<img src='cid:star_01' alt='Star'/>\r\n</body>\r\n</html>";

        private static readonly string SenderName = "sender";
        private static readonly string SenderMail = "sendender@teste.com";
        private static readonly string ReceiverMail = "reciever@test.com";
        private static readonly string Subject = "Test Mail";
        private static readonly string TextBody = "Test Text";

        private static readonly string EmailsDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\mail_test";

        [Fact]
        public void Should_send_mail_with_subject_only()
        {
            UnityTestEnviromentChecker.OnTestEnviroment = true;

            EmailHandler.SendEmail(
                "smtp.test.com",
                false,
                587,
                new MailAddress(SenderMail, SenderName),
                new MailAddress(ReceiverMail),
                Subject);

            var mailString = File.ReadAllText(Directory.GetFiles(EmailsDirectory).First());
            mailString.Contains($"X-Sender: \"{SenderName}\" <{SenderMail}>").ShouldBeTrue();
            mailString.Contains($"X-Receiver: {ReceiverMail}").ShouldBeTrue();
            mailString.Contains($"From: \"{SenderName}\" <{SenderMail}>").ShouldBeTrue();
            mailString.Contains($"To: {ReceiverMail}").ShouldBeTrue();
            mailString.Contains($"Subject: {Subject}").ShouldBeTrue();
        }

            [Fact]
        public void Should_send_mail_with_attachments()
        {
            UnityTestEnviromentChecker.OnTestEnviroment = true;

            EmailHandler.SendEmail(
                "smtp.test.com",
                false,
                587,
                new MailAddress(SenderMail, SenderName),
                new MailAddress(ReceiverMail),
                Subject,
                TextBody,
                BasicHtml,
                null,
                new MailAttachment[] { new MailAttachment(new ContentType("image/png"), StarImageBase64) },
                new NetworkCredential("test", "password"));

            var mailString = File.ReadAllText(Directory.GetFiles(EmailsDirectory).First());

            mailString.Contains($"X-Sender: \"{SenderName}\" <{SenderMail}>").ShouldBeTrue();
            mailString.Contains($"X-Receiver: {ReceiverMail}").ShouldBeTrue();
            mailString.Contains($"From: \"{SenderName}\" <{SenderMail}>").ShouldBeTrue();
            mailString.Contains($"To: {ReceiverMail}").ShouldBeTrue();
            mailString.Contains($"Subject: {Subject}").ShouldBeTrue();
            mailString.Contains(TextBody).ShouldBeTrue();

            CheckStringInChunks(mailString, StarImageBase64, 68, 1);

            var base64HtmlBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(BasicHtml));
            CheckStringInChunks(mailString, base64HtmlBody, 68, 1);
        }

        [Fact]
        public void Should_send_mail_with_attachments_and_image()
        {
            UnityTestEnviromentChecker.OnTestEnviroment = true;

            EmailHandler.SendEmail(
                "smtp.test.com",
                false,
                587,
                new MailAddress(SenderMail, SenderName),
                new MailAddress(ReceiverMail),
                Subject,
                TextBody,
                InlineHtml,
                new InlineImage[] { new InlineImage("star_01", new ContentType("image/png"), StarImageBase64) },
                new MailAttachment[] { new MailAttachment(new ContentType("image/png"), StarImageBase64) },
                new NetworkCredential("test", "password"));

            var mailString = File.ReadAllText(Directory.GetFiles(EmailsDirectory).First());

            mailString.Contains($"X-Sender: \"{SenderName}\" <{SenderMail}>").ShouldBeTrue();
            mailString.Contains($"X-Receiver: {ReceiverMail}").ShouldBeTrue();
            mailString.Contains($"From: \"{SenderName}\" <{SenderMail}>").ShouldBeTrue();
            mailString.Contains($"To: {ReceiverMail}").ShouldBeTrue();
            mailString.Contains($"Subject: {Subject}").ShouldBeTrue();
            mailString.Contains(TextBody).ShouldBeTrue();

            CheckStringInChunks(mailString, StarImageBase64, 68, 2);

            var base64HtmlBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(InlineHtml));
            CheckStringInChunks(mailString, base64HtmlBody, 68, 1);
        }

        private static void CheckStringInChunks(string mainString, string ToCheckString, int maxChunkSize, int occurences)
        {
            foreach (var base64Chunk in GetChunksUpto(ToCheckString, maxChunkSize))
            {
                Regex.Matches(mainString, Regex.Escape(base64Chunk)).Count.ShouldBe(occurences);
            }
        }
        
        private static IEnumerable<string> GetChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }
        
        public void Dispose()
        {
            if (Directory.Exists(EmailsDirectory))
            {
                Directory.Delete(EmailsDirectory, true);
            }

            GC.SuppressFinalize(this);
        }
    }
}
