using Allure.Net.Commons;
using NUnit.Allure.Core;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [SetUpFixture]
    public class AllureSetUpFixture
    {
        [OneTimeSetUp]
        public static void CleanupResultDirectory() =>
            AllureExtensions.WrapSetUpTearDownParams(
                AllureLifecycle.Instance.CleanupResultDirectory,
                "Clear Allure Results Directory"
            );
    }
}
