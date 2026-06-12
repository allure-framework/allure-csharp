using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public sealed class AllureTestUpdateMessage(
    SessionUid sessionUid,
    string testUid
) :
    MutateModelMessage(
        "Allure test result update",
        "This message reports that some data needs to be associated with an Allure test result.",
        sessionUid,
        testUid
    )
{
    public string TestUid { get; } = testUid;

    public override void Mutate(IAllureInfrastructure allure)
    {
        allure.Lifecycle.UpdateTestCase((test) =>
        {
            this.ApplyProperties(allure, test);
        });
    }
}