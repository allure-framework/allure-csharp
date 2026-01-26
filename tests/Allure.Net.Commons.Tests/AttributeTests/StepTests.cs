using System.Threading.Tasks;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Steps;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

public class StepTests
{
    [Test]
    public void CreatesStepFromVoidMethodCall()
    {
        var tr = new TestResult();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithTestContext(tr),
            VoidMethod
        );

        Assert.That(tr.steps, Has.One.Items);
        var step = tr.steps[0];
        Assert.That(step.name, Is.EqualTo(nameof(VoidMethod)));
    }

    [Test]
    public void CreatesStepFromFunctionCall()
    {
        var tr = new TestResult();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithTestContext(tr),
            () => MethodReturningInt()
        );

        Assert.That(tr.steps, Has.One.Items);
        var step = tr.steps[0];
        Assert.That(step.name, Is.EqualTo(nameof(MethodReturningInt)));
    }

    [Test]
    public async Task CreatesStepFromAsyncMethodCall()
    {
        var tr = new TestResult();

        await AllureLifecycle.Instance.RunInContextAsync(
            new AllureContext().WithTestContext(tr),
            AsyncMethod
        );

        Assert.That(tr.steps, Has.One.Items);
        var step = tr.steps[0];
        Assert.That(step.name, Is.EqualTo(nameof(AsyncMethod)));
    }

    [Test]
    public async Task CreatesStepFromAsyncFunctionCall()
    {
        var tr = new TestResult();

        await AllureLifecycle.Instance.RunInContextAsync(
            new AllureContext().WithTestContext(tr),
            AsyncMethodReturningInt
        );

        Assert.That(tr.steps, Has.One.Items);
        var step = tr.steps[0];
        Assert.That(step.name, Is.EqualTo(nameof(AsyncMethodReturningInt)));
    }

    [Test]
    public void CreatesNamedStep()
    {
        var tr = new TestResult();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithTestContext(tr),
            NamedStep
        );

        Assert.That(tr.steps, Has.One.Items);
        var step = tr.steps[0];
        Assert.That(step.name, Is.EqualTo("Foo"));
    }

    [Test]
    public void CreatesStepWithParameters()
    {
        var tr = new TestResult();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithTestContext(tr),
            () => StepWithParameters(1, "baz")
        );

        Assert.That(tr.steps, Has.One.Items);
        var step = tr.steps[0];
        Assert.That(
            step.parameters,
            Is.EqualTo([
                new Parameter { name = "foo", value = "1" },
                new Parameter { name = "bar", value = "\"baz\"" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void InterpolatesParameterIntoStepName()
    {
        var tr = new TestResult();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithTestContext(tr),
            () => Interpolation("bar")
        );

        Assert.That(tr.steps, Has.One.Items);
        var step = tr.steps[0];
        Assert.That(step.name, Is.EqualTo("Foo \"bar\""));
    }

    [Test]
    public void DoesntAddIgnoredParameters()
    {
        var tr = new TestResult();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithTestContext(tr),
            () => IgnoredParameter("bar")
        );

        Assert.That(tr.steps, Has.One.Items);
        var step = tr.steps[0];
        Assert.That(step.parameters, Is.Empty);
    }

    [Test]
    public void AssignsCustomNameToParameter()
    {
        var tr = new TestResult();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithTestContext(tr),
            () => RenamedParameter("baz")
        );

        Assert.That(tr.steps, Has.One.Items);
        var parameter = tr.steps[0].parameters[0];
        Assert.That(parameter.name, Is.EqualTo("bar"));
    }

    [Test]
    public void UsesOriginalParameterNameForInterpolation()
    {
        var tr = new TestResult();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithTestContext(tr),
            () => InterpolatedRenamedParameter("baz")
        );

        Assert.That(tr.steps[0].name, Is.EqualTo("\"baz\""));
    }

    [AllureStep]
    static void VoidMethod() { }

    [AllureStep]
    static int MethodReturningInt() => default;

    [AllureStep]
    static async Task AsyncMethod()
    {
        await Task.Delay(1);
    }

    [AllureStep]
    static async Task<int> AsyncMethodReturningInt()
    {
        await Task.Delay(1);
        return 1;
    }

    [AllureStep("Foo")]
    static void NamedStep() { }

    [AllureStep]
    static void StepWithParameters(int foo, string bar) { }

    [AllureStep("Foo {foo}")]
    static void Interpolation(string foo) { }

    [AllureStep]
    static void IgnoredParameter([AllureParameter(Ignore = true)] string foo) { }

    [AllureStep]
    static void RenamedParameter([AllureParameter(Name = "bar")] string foo) { }

    [AllureStep("{foo}")]
    static void InterpolatedRenamedParameter([AllureParameter(Name = "bar")] string foo) { }
}