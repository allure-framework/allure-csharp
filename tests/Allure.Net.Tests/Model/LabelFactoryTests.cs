using System.Globalization;
using Allure.Model;

namespace Allure.Net.Tests.Model;

public class LabelFactoryTests
{
    [Test]
    public async Task NamedFactoriesProduceExpectedLabelPairs()
    {
        var labels = new[]
        {
            Label.Create("custom", "value"),
            Label.AllureId(42),
            Label.Suite("suite"),
            Label.ParentSuite("parent"),
            Label.SubSuite("sub-suite"),
            Label.Epic("epic"),
            Label.Feature("feature"),
            Label.Story("story"),
            Label.Tag("tag"),
            Label.Owner("owner"),
            Label.Lead("lead"),
            Label.Host("host"),
            Label.Thread("thread"),
            Label.TestMethod("method"),
            Label.TestClass("class"),
            Label.Package("package"),
            Label.Framework("framework"),
            Label.Language("language"),
            Label.Layer("layer"),
        };

        await Assert.That(labels.Select(label => (label.Name, label.Value)))
            .IsEquivalentTo(
                [
                    ("custom", "value"),
                    (LabelName.AllureId, "42"),
                    (LabelName.Suite, "suite"),
                    (LabelName.ParentSuite, "parent"),
                    (LabelName.SubSuite, "sub-suite"),
                    (LabelName.Epic, "epic"),
                    (LabelName.Feature, "feature"),
                    (LabelName.Story, "story"),
                    (LabelName.Tag, "tag"),
                    (LabelName.Owner, "owner"),
                    (LabelName.Lead, "lead"),
                    (LabelName.Host, "host"),
                    (LabelName.Thread, "thread"),
                    (LabelName.TestMethod, "method"),
                    (LabelName.TestClass, "class"),
                    (LabelName.Package, "package"),
                    (LabelName.Framework, "framework"),
                    (LabelName.Language, "language"),
                    (LabelName.Layer, "layer"),
                ]
            );
    }

    [Test]
    public async Task SeverityFactoryProducesCanonicalWireValues()
    {
        var labels = Enum.GetValues<Severity>().Select(Label.Severity).ToArray();

        await Assert.That(labels.Select(label => label.Name))
            .IsEquivalentTo(Enumerable.Repeat(LabelName.Severity, labels.Length));
        await Assert.That(labels.Select(label => label.Value))
            .IsEquivalentTo(["blocker", "critical", "normal", "minor", "trivial"]);
    }

    [Test]
    public async Task SeverityFactoryRejectsUnknownValues()
    {
        await Assert.That(() => Label.Severity((Severity)int.MaxValue))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task EnvironmentFactoriesUseCurrentEnvironment()
    {
        var expectedThread = Thread.CurrentThread.Name
            ?? Thread.CurrentThread.ManagedThreadId.ToString();

        var host = Label.Host();
        var thread = Label.Thread();
        var language = Label.Language();

        await Assert.That((host.Name, host.Value))
            .IsEqualTo((LabelName.Host, Environment.MachineName));
        await Assert.That((thread.Name, thread.Value))
            .IsEqualTo((LabelName.Thread, expectedThread));
        await Assert.That((language.Name, language.Value))
            .IsEqualTo((LabelName.Language, "C#"));
    }

    [Test]
    public async Task AllureIdFactoryIsCultureIndependent()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NegativeSign = "custom-minus";

        try
        {
            CultureInfo.CurrentCulture = culture;
            var label = Label.AllureId(-42);

            await Assert.That(label.Value).IsEqualTo("-42");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
