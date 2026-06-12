using Allure.TestingPlatform.Properties;

namespace Allure.TestingPlatform.Tests.FixtureTests;

[InheritsTests]
public class StopFixturePropertyTests : FixturePropertyTestBase
{
    protected override List<IAllureProperty> PropertyListSelector =>
        this.StopFixtureMessage.Properties;
}
