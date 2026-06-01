using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.NUnit.Tests.Fixtures;

class FixtureTests
{
    [Test]
    [Skip("Can't emit OneTime-fixture container: need sdk hook")]
    public async Task FixtureAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.FixtureAttributes);

        var uuid = await Assert.That(results).HasSingleTestResult().With.Uuid();
        await Assert.That(results.Containers).Count().IsEqualTo(2);
        await Assert.That(results)
            .HasSingleContainer("Allure.NUnit.Tests.Fixtures.Samples.LegacyFixtureAttributes.TestsClass")
            .With.BeforesMatching(
                [fixture => fixture.HasName("OneTimeSetUp")
                    .And.HasStatus(AllureStatus.Passed)]);

        await Assert.That(results)
            .HasSingleContainer("Allure.NUnit.Tests.Fixtures.Samples.LegacyFixtureAttributes.TestsClass.TestMethod")
            .With.OnlyOneBeforeFixture(
                fixture => fixture.HasName("Foo")
                    .And.HasStatus(AllureStatus.Passed))
            .With.OnlyOneAfterFixture(
                fixture => fixture.HasName("TearDown")
                    .And.HasStatus(AllureStatus.Passed))
            .With.SingleChild().That.IsEqualTo(uuid);
    }

    [Test]
    public async Task LegacyFixtureAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LegacyFixtureAttributes);

        var uuid = await Assert.That(results).HasSingleTestResult().With.Uuid();
        await Assert.That(results.Containers).Count().IsEqualTo(2);
        await Assert.That(results)
            .HasSingleContainer("Allure.NUnit.Tests.Fixtures.Samples.LegacyFixtureAttributes.TestsClass")
            .With.OnlyOneBeforeFixture(
                fixture => fixture.HasName("OneTimeSetUp")
                    .And.HasStatus(AllureStatus.Passed))
            .With.OnlyOneAfterFixture(
                fixture => fixture.HasName("Bar")
                    .And.HasStatus(AllureStatus.Passed))
            .With.SingleChild().That.IsEqualTo(uuid);
        await Assert.That(results)
            .HasSingleContainer("Allure.NUnit.Tests.Fixtures.Samples.LegacyFixtureAttributes.TestsClass.TestMethod")
            .With.OnlyOneBeforeFixture(
                fixture => fixture.HasName("Foo")
                    .And.HasStatus(AllureStatus.Passed))
            .With.OnlyOneAfterFixture(
                fixture => fixture.HasName("TearDown")
                    .And.HasStatus(AllureStatus.Passed))
            .With.SingleChild().That.IsEqualTo(uuid);
    }
}
