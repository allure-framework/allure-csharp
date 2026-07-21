using Allure.Abstractions;
using Allure.Model;
using ModelTestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Metadata;

public class MetadataDiscoveryTests
{
    [Test]
    public async Task TypeMetadataIsAppliedFromInterfaceThroughDerivedType()
    {
        var result = CreateResult();

        AllureApiAttribute.ApplyTypeAttributes(typeof(DerivedFixture), result);

        await Assert.That(result.Description)
            .IsEqualTo("interface\n\nbase\n\nderived");
    }

    [Test]
    public async Task TypeNameIsNotAppliedToTestResult()
    {
        var result = CreateResult();

        AllureApiAttribute.ApplyTypeAttributes(typeof(NamedFixture), result);

        await Assert.That(result.Name).IsEqualTo("original");
        await Assert.That(result.Labels.Single().Value).IsEqualTo("type-label");
    }

    [Test]
    public async Task MethodMetadataIsAppliedFromBaseOverrideThroughDerivedOverride()
    {
        var result = CreateResult();
        var method = typeof(DerivedFixture).GetMethod(nameof(DerivedFixture.Run))!;

        AllureApiAttribute.ApplyMethodAttributes(method, result);

        await Assert.That(result.Description)
            .IsEqualTo("base method\n\nderived method");
    }

    [Test]
    public async Task AllMetadataAppliesTypeBeforeMethodMetadata()
    {
        var result = CreateResult();
        var method = typeof(DerivedFixture).GetMethod(nameof(DerivedFixture.Run))!;

        AllureApiAttribute.ApplyAllAttributes(method, result);

        await Assert.That(result.Description)
            .IsEqualTo(
                "interface\n\nbase\n\nderived\n\nbase method\n\nderived method"
            );
    }

    static ModelTestResult CreateResult() => new() { Uuid = "test-id", Name = "original" };

    [AllureDescription("interface", Append = true)]
    interface IFixtureMetadata;

    [AllureDescription("base", Append = true)]
    class BaseFixture : IFixtureMetadata
    {
        [AllureDescription("base method", Append = true)]
        public virtual void Run() { }
    }

    [AllureDescription("derived", Append = true)]
    sealed class DerivedFixture : BaseFixture
    {
        [AllureDescription("derived method", Append = true)]
        public override void Run() { }
    }

    [AllureName("type name")]
    [AllureLabel("source", "type-label")]
    sealed class NamedFixture;
}
