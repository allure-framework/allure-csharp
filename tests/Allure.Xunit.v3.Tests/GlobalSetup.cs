[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

#if ALLURE_TEST_PARALLEL

[assembly: ParallelLimiter<Allure.Testing.DotnetParallelLimit>]

#else

[assembly: NotInParallel(["Allure.Xunit.v3", "Allure.TestingPlatform", "Allure.Net.Commons"])]

#endif

namespace Allure.Xunit.v3.Tests;
