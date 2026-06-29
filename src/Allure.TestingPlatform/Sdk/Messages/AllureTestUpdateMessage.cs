using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

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

    public override void Mutate(ReadyAllureTestingPlatformRuntime allureState)
    {
        allureState.Lifecycle.UpdateTestCase((test) =>
        {
            this.ApplyProperties(allureState, test);
        });
    }
}