using rbkApiModules.Utilities.Avatar;
using Shouldly;
using Xunit;

namespace rbkApiModules.Utilities.Tests
{
    public class AvatarGenerationTests
    {
        [Fact]
        public void Should_Generate_Avatar_With_One_Initial()
        {
            var svg = AvatarGenerator.GenerateSvgAvatar("Rafael de Oliveira Basniak");

            svg.Contains(">RB<").ShouldBeTrue();

            svg.Contains("{{COLOR}}").ShouldBeFalse();
            svg.Contains("{{INITIALS}}").ShouldBeFalse();
        }

        [Fact]
        public void Should_Generate_Avatar_With_Two_Initials()
        {
            var svg = AvatarGenerator.GenerateSvgAvatar("Bruna");

            svg.Contains(">B<").ShouldBeTrue();

            svg.Contains("{{COLOR}}").ShouldBeFalse();
            svg.Contains("{{INITIALS}}").ShouldBeFalse();
        }

        [Fact]
        public void Should_Encode_Avatar()
        {
            var data = AvatarGenerator.GenerateBase64Avatar("Bruna");

            data.StartsWith("data:image/svg+xml;base64,").ShouldBeTrue();
        }
    }
}
