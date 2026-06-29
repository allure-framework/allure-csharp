using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureTestUpdateMessage(
    CorrelationUid correlationUid,
    TestContextUid testUid
) :
    MutateModelMessage(
        "Allure test result update",
        "This message reports that some data needs to be associated with an Allure test result.",
        correlationUid,
        testUid
    )
{
    public TestContextUid TestUid { get; } = testUid;

    public override void Mutate(LiveAllureTestingPlatformRuntime allureState)
    {
        allureState.Lifecycle.UpdateTestCase((test) =>
        {
            this.ApplyProperties(allureState, test);
        });
    }
}