using Allure.Testing;

namespace Allure.Xunit.Tests.CustomLabels;

class CustomLabelTests
{
    [Test]
    public async Task CheckCustomLabelIsAdded()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.AddLabelApi);

        await Assert.That(results).HasSingleTestResult()
            .With.Label(l => l.HasName("test").And.HasValue("foo"))
            .With.Label(l => l.HasName("dispose").And.HasValue("bar"));
    }

    [Test]
    public async Task LabelAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LabelAttribute);

        await Assert.That(results).HasSingleTestResult()
            .With.Label(l => l.HasName("interface").And.HasValue("foo"))
            .With.Label(l => l.HasName("baseClass").And.HasValue("bar"))
            .With.Label(l => l.HasName("class").And.HasValue("baz"))
            .With.Label(l => l.HasName("method").And.HasValue("qux"));
    }

    [Test]
    public async Task LegacyLabelAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LegacyLabelAttribute);

        await Assert.That(results).HasSingleTestResult()
            .With.Label(l => l.HasName("class").And.HasValue("bar"))
            .With.Label(l => l.HasName("method").And.HasValue("baz"));
    }
}
