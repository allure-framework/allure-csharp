// Here you could define global logic that would affect all tests

[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

[assembly: NotInParallel(["Allure.NUnit", "Allure.Net.Commons"])]

namespace Allure.NUnit.Tests;

public class GlobalHooks
{
    [Before(TestSession)]
    public static void SetUp()
    {

    }

    [After(TestSession)]
    public static void CleanUp()
    {

    }
}
