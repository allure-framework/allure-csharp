[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

#if !ALLURE_TEST_PARALLEL

[assembly: NotInParallel(["Allure.NUnit", "Allure.Net.Commons"])]

#endif

namespace Allure.NUnit.Tests;
