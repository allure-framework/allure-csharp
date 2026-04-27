using Allure.Testing;

[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

#if ALLURE_TEST_PARALLEL

[assembly: ParallelLimiter<DotnetParallelLimit>]

#else

[assembly: NotInParallel(["Allure.NUnit", "Allure.Net.Commons"])]

#endif

namespace Allure.Xunit.v3.Tests;




