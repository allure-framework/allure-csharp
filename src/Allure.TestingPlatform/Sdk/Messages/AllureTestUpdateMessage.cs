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

    public override void Mutate(IAllureRuntime allure)
    {
        allure.Lifecycle.UpdateTestCase((test) =>
        {
            this.ApplyProperties(allure, test);
        });
    }
}