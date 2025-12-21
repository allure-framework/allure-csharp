using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [SetUpFixture]
    public class AllureSetUpFixture
    {
        [OneTimeSetUp]
        public static void CleanupResultDirectory() =>
            AllureLifecycle.Instance.CleanupResultDirectory();
    }
}
