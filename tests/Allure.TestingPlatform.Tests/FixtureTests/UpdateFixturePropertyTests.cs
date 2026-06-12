using Allure.TestingPlatform.Properties;

namespace Allure.TestingPlatform.Tests.FixtureTests;

[InheritsTests]
public class UpdateFixturePropertyTests : FixturePropertyTestBase
{
    protected override List<IAllureProperty> PropertyListSelector =>
        this.UpdateFixtureMessage.Properties;
}
