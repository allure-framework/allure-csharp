using Xunit;
using Xunit.Sdk;

namespace Allure.Xunit.Attributes
{
    [XunitTestCaseDiscoverer("Allure.Xunit.AllureXunitDiscover", "Allure.Xunit")]
    public class AllureXunitAttribute : FactAttribute
    {
    }
}