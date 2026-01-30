using System.Threading.Tasks;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

public class AfterFixtureTests
{
    [Test]
    public void CreatesFixtureFromVoidMethodCall()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            VoidMethod
        );

        Assert.That(tc.afters, Has.One.Items);
        var fixture = tc.afters[0];
        Assert.That(fixture.name, Is.EqualTo(nameof(VoidMethod)));
    }

    [Test]
    public void CreatesFixtureFromFunctionCall()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => MethodReturningInt()
        );

        Assert.That(tc.afters, Has.One.Items);
        var fixture = tc.afters[0];
        Assert.That(fixture.name, Is.EqualTo(nameof(MethodReturningInt)));
    }

    [Test]
    public async Task CreateFixtureFromAsyncMethodCall()
    {
        var tc = new TestResultContainer();

        await AllureLifecycle.Instance.RunInContextAsync(
            new AllureContext().WithContainer(tc),
            AsyncMethod
        );

        Assert.That(tc.afters, Has.One.Items);
        var fixture = tc.afters[0];
        Assert.That(fixture.name, Is.EqualTo(nameof(AsyncMethod)));
    }

    [Test]
    public async Task CreateFixtureFromAsyncFunctionCall()
    {
        var tc = new TestResultContainer();

        await AllureLifecycle.Instance.RunInContextAsync(
            new AllureContext().WithContainer(tc),
            AsyncMethodReturningInt
        );

        Assert.That(tc.afters, Has.One.Items);
        var fixture = tc.afters[0];
        Assert.That(fixture.name, Is.EqualTo(nameof(AsyncMethodReturningInt)));
    }

    [Test]
    public void CreatesNamedFixture()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            NamedFixture
        );

        Assert.That(tc.afters, Has.One.Items);
        var fixture = tc.afters[0];
        Assert.That(fixture.name, Is.EqualTo("Foo"));
    }

    [Test]
    public void CreateFixtureWithParameters()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => FixtureWithParameters(1, "baz")
        );

        Assert.That(tc.afters, Has.One.Items);
        var fixture = tc.afters[0];
        Assert.That(
            fixture.parameters,
            Is.EqualTo([
                new Parameter { name = "foo", value = "1" },
                new Parameter { name = "bar", value = "\"baz\"" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void InterpolatesParameterIntoFixtureName()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => Interpolation("bar")
        );

        Assert.That(tc.afters, Has.One.Items);
        var fixture = tc.afters[0];
        Assert.That(fixture.name, Is.EqualTo("Foo \"bar\""));
    }

    [Test]
    public void IgnoredParametersNotAdded()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => IgnoredParameter("bar")
        );

        Assert.That(tc.afters, Has.One.Items);
        var fixture = tc.afters[0];
        Assert.That(fixture.parameters, Is.Empty);
    }

    [Test]
    public void AssignsCustomNameToParameter()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => RenamedParameter("baz")
        );

        Assert.That(tc.afters, Has.One.Items);
        var parameter = tc.afters[0].parameters[0];
        Assert.That(parameter.name, Is.EqualTo("bar"));
    }

    [Test]
    public void UsesOriginalParameterNameForInterpolation()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => InterpolatedRenamedParameter("baz")
        );

        Assert.That(tc.afters[0].name, Is.EqualTo("\"baz\""));
    }

    [Test]
    public void AppliesMaskedModeToParameter()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => MaskedParameter("foo")
        );

        Assert.That(tc.afters[0].parameters[0].mode, Is.EqualTo(ParameterMode.Masked));
    }

    [Test]
    public void AppliesHiddenModeToParameter()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => HiddenParameter("foo")
        );

        Assert.That(tc.afters[0].parameters[0].mode, Is.EqualTo(ParameterMode.Hidden));
    }

    [AllureAfter]
    static void VoidMethod() { }

    [AllureAfter]
    static int MethodReturningInt() => default;

    [AllureAfter]
    static async Task AsyncMethod()
    {
        await Task.Delay(1);
    }

    [AllureAfter]
    static async Task<int> AsyncMethodReturningInt()
    {
        await Task.Delay(1);
        return 1;
    }

    [AllureAfter("Foo")]
    static void NamedFixture() { }

    [AllureAfter]
    static void FixtureWithParameters(int foo, string bar) { }

    [AllureAfter("Foo {foo}")]
    static void Interpolation(string foo) { }

    [AllureAfter]
    static void IgnoredParameter([AllureParameter(Ignore = true)] string foo) { }

    [AllureAfter]
    static void RenamedParameter([AllureParameter(Name = "bar")] string foo) { }

    [AllureAfter("{foo}")]
    static void InterpolatedRenamedParameter([AllureParameter(Name = "bar")] string foo) { }

    [AllureAfter]
    static void MaskedParameter([AllureParameter(Mode = ParameterMode.Masked)] string foo) { }

    [AllureAfter]
    static void HiddenParameter([AllureParameter(Mode = ParameterMode.Hidden)] string foo) { }
}