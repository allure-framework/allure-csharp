[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

#if ALLURE_TEST_PARALLEL

[assembly: ParallelLimiter<Allure.Testing.DotnetParallelLimit>]

#else

[assembly: NotInParallel(["Allure.NUnit", "Allure.Net.Commons"])]

#endif

namespace Allure.Xunit.Tests;
