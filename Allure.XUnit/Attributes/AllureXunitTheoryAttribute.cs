using Xunit;
using Xunit.Sdk;

namespace Allure.Xunit.Attributes
{
    [XunitTestCaseDiscoverer("Allure.Xunit.AllureXunitTheoryDiscover", "Allure.Xunit")]
    public class AllureXunitTheoryAttribute : FactAttribute
    {
    }
}