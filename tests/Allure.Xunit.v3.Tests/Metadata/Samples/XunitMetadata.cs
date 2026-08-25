using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata
{
    public class DefaultMetadataClass
    {
        [Fact]
        public void PlainFact() { }

        [Theory]
        [InlineData("foo")]
        public void PlainTheory(string value)
        {
            Assert.NotEmpty(value);
        }
    }
}
