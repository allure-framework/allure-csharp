using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;
using Allure.Runtime;

namespace Allure.Net.Tests.ApiContract;

public class PublicApiSnapshotTests
{
    [Test]
    public async Task FacadeSurfaceMatchesSnapshot()
    {
        var snapshot = PublicApiSnapshot.Create(typeof(AllureApi), typeof(AllureInProcessApi));

        await Assert.That(PublicApiSnapshot.Hash(snapshot))
            .IsEqualTo("503F65D233A49E8E7B6468AB7BA1B282A0DD49F682D39F6783340E701414C810")
            .Because(snapshot);
    }

    [Test]
    public async Task RuntimeAndAbstractionSurfaceMatchesSnapshot()
    {
        var assembly = typeof(IAllureApiClient).Assembly;
        var types = assembly.GetExportedTypes()
            .Where(type => type.Namespace == typeof(IAllureApiClient).Namespace && (
                type.IsInterface || type.Name is nameof(AllureApiOperations) or nameof(AllureRuntimeOperations)
            ))
            .Concat([
                typeof(AllureFrontend),
                typeof(AllureBackend),
                typeof(AllureFrontendState),
                typeof(AllureRuntimeRegistry),
            ]);
        var snapshot = PublicApiSnapshot.Create(types);

        await Assert.That(PublicApiSnapshot.Hash(snapshot))
            .IsEqualTo("2437397619BE201C7658D9551BD542A552F30DF5EFA1C8585787177899269B28")
            .Because(snapshot);
    }

    [Test]
    public async Task RuntimeAttributeSurfaceMatchesSnapshot()
    {
        var snapshot = PublicApiSnapshot.Create(
            typeof(AllureStepAttribute),
            typeof(AllureBeforeAttribute),
            typeof(AllureAfterAttribute),
            typeof(AllureAttachmentAttribute),
            typeof(AllureAttachmentFileAttribute),
            typeof(AllureParameterAttribute)
        );

        await Assert.That(PublicApiSnapshot.Hash(snapshot))
            .IsEqualTo("0E9483609088A0ECE9675616BC2C15BF891507F1A5986135829E014BDEEC17DC")
            .Because(snapshot);
    }
}
