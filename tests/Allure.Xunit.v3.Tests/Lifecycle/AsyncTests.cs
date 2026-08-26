using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Lifecycle;

class AsyncTests
{
    readonly static AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        var output = await AllureSampleRunner.RunAsync(AllureSampleRegistry.AsyncTests, token);

        await Assert.That(output.TestResults).Count().IsEqualTo(6);

        results.Value = output;
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task ShouldReportAllAsyncTests()
    {
        await Assert.That(results.Value).HasTestResults([
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.AsyncTests.AsyncTestClass1.AsyncFact1"),
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.AsyncTests.AsyncTestClass1.AsyncTheory1(value: \"foo\")")
                .With.SingleParameter("value").That.HasValue("\"foo\""),
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.AsyncTests.AsyncTestClass1.AsyncTheory1(value: \"bar\")")
                .With.SingleParameter("value").That.HasValue("\"bar\""),
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.AsyncTests.AsyncTestClass2.AsyncFact2"),
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.AsyncTests.AsyncTestClass2.AsyncTheory2(value: \"foo\")")
                .With.SingleParameter("value").That.HasValue("\"foo\""),
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.Lifecycle.AsyncTests.AsyncTestClass2.AsyncTheory2(value: \"bar\")")
                .With.SingleParameter("value").That.HasValue("\"bar\""),
        ]);
    }
}
