using System.Reflection;
using Allure.Model;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureSeverityAttributeTests
{
    [Test]
    public async Task SeverityOfMostDerivedClassIsQueried()
    {
        var attr = typeof(Derived).GetCustomAttributes<AllureSeverityAttribute>().Single();
        await Assert.That(attr.Value).IsEqualTo("blocker");
    }

    [Test]
    public async Task SeverityCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureSeverityAttribute(Severity.Critical);

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("severity");
        await Assert.That(attr.Value).IsEqualTo("critical");
        await AttributeAssertions.AssertLabels(tr, ("severity", "critical"));
    }

    [AllureSeverity(Severity.Normal)]
    private class Base;
    [AllureSeverity(Severity.Blocker)]
    private class Derived : Base;
}
