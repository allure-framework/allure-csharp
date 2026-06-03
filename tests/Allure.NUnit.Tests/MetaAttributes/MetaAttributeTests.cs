using Allure.Testing;

namespace Allure.NUnit.Tests.MetaAttributes;

class MetaAttributeTests
{
    [Test]
    public async Task MetaAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.MetaAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.OnlyOneLabel(l => l.HasName("epic").And.HasValue("Foo"))
            .With.OnlyOneLabel(l => l.HasName("owner").And.HasValue("John Doe"))
            .With.OnlyOneLabel(l => l.HasName("feature").And.HasValue("Bar"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("foo"))
            .With.OnlyOneLabel(l => l.HasName("tag").And.HasValue("bar"))
            .With.OnlyOneLabel(l => l.HasName("story").And.HasValue("Baz"))
            .With.OnlyOneLabel(l => l.HasName("severity").And.HasValue("critical"))
            .With.OnlyOneLabel(l => l.HasName("suite").And.HasValue("Qux"))
            .With.SingleLink(
                link => link.HasUrl("https://foo.bar/")
                    .And.HasNoName()
                    .And.HasNoType()
            );
    }
}
