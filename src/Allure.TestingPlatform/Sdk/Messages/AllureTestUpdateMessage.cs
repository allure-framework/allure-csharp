using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports updates for an Allure test result.
/// </summary>
public sealed class AllureTestUpdateMessage(
    CorrelationUid correlationUid,
    TestContextUid testUid
) :
    AllureModelUpdateMessage(
        "Allure test result update",
        "This message reports that some data needs to be associated with an Allure test result.",
        correlationUid,
        testUid
    )
{
    /// <summary>
    /// Gets the test context identifier.
    /// </summary>
    public TestContextUid TestUid { get; } = testUid;

    /// <inheritdoc />
    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime)
    {
        allureRuntime.Lifecycle.UpdateTestCase((test) =>
        {
            this.ApplyProperties(allureRuntime, test);
        });
    }
}
