using Allure.TestingPlatform.Sdk.Properties;

namespace Allure.TestingPlatform.Tests.FixtureTests;

[InheritsTests]
public class StartFixturePropertyTests : FixturePropertyTestBase
{
    protected override List<IAllureProperty> PropertyListSelector =>
        this.StartFixtureMessage.Properties;
}
