using Allure.Testing;
using Allure.Testing.Assertions.Model;
using TUnit.Assertions.Enums;

namespace Allure.Xunit.v3.Tests.Metadata;

class XunitMetadataTests
{
    readonly static AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.XunitMetadata, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(2);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task CheckFactMethodIdentityMetadataIsMapped()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(
                "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata.DefaultMetadataClass.PlainFact")
            .With.FullName(
                "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata:"
                    + "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata."
                    + "DefaultMetadataClass.PlainFact()")
            .With.TitlePath((tp) => tp.IsEquivalentTo(
                [
                    "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata",
                    "Allure",
                    "Xunit",
                    "v3",
                    "Tests",
                    "Samples",
                    "Metadata",
                    "XunitMetadata",
                    "DefaultMetadataClass",
                ],
                CollectionOrdering.Matching));
        await Assert.That(testResult).HasSingleLabel("testClass").With.Value("DefaultMetadataClass");
        await Assert.That(testResult).HasSingleLabel("testMethod").With.Value("PlainFact");
        await Assert.That(testResult).HasSingleLabel("package").With.Value(
            "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata.DefaultMetadataClass"
        );
    }

    [Test]
    public async Task CheckTheoryMethodIdentityMetadataIsMapped()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(
                "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata.DefaultMetadataClass.PlainTheory(value: \"foo\")")
            .With.FullName(
                "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata:"
                    + "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata."
                    + "DefaultMetadataClass.PlainTheory(System.String)")
            .With.TitlePath((tp) => tp.IsEquivalentTo(
                [
                    "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata",
                    "Allure",
                    "Xunit",
                    "v3",
                    "Tests",
                    "Samples",
                    "Metadata",
                    "XunitMetadata",
                    "DefaultMetadataClass",
                    "PlainTheory(System.String)",
                ],
                CollectionOrdering.Matching));
        await Assert.That(testResult).HasSingleLabel("testClass").With.Value("DefaultMetadataClass");
        await Assert.That(testResult).HasSingleLabel("testMethod").With.Value("PlainTheory");
        await Assert.That(testResult).HasSingleLabel("package").With.Value(
            "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata.DefaultMetadataClass"
        );
    }

    [Test]
    public async Task CheckDefaultSuiteLabelsAreMappedFromXunitMetadata()
    {
        var testResult = await Assert.That(results.Value).HasSingleTestResult(
            "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata.DefaultMetadataClass.PlainFact"
        );

        await Assert.That(testResult).HasSingleLabel("parentSuite").With.Value(
            "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata"
        );
        await Assert.That(testResult).HasSingleLabel("suite").With.Value(
            "Allure.Xunit.v3.Tests.Samples.Metadata.XunitMetadata"
        );
        await Assert.That(testResult).HasSingleLabel("subSuite").With.Value("DefaultMetadataClass");
    }
}
