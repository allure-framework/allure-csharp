using System.Threading.Tasks;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

public class BeforeFixtureTests
{
    [Test]
    public void CreatesFixtureFromVoidMethodCall()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            VoidMethod
        );

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
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

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
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

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
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

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
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

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
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

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
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

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
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

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
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

        Assert.That(tc.befores, Has.One.Items);
        var parameter = tc.befores[0].parameters[0];
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

        Assert.That(tc.befores[0].name, Is.EqualTo("\"baz\""));
    }

    [Test]
    public void AppliesMaskedModeToParameter()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => MaskedParameter("foo")
        );

        Assert.That(tc.befores[0].parameters[0].mode, Is.EqualTo(ParameterMode.Masked));
    }

    [Test]
    public void AppliesHiddenModeToParameter()
    {
        var tc = new TestResultContainer();

        AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            () => HiddenParameter("foo")
        );

        Assert.That(tc.befores[0].parameters[0].mode, Is.EqualTo(ParameterMode.Hidden));
    }

    [Test]
    public void CanBeAppliedOnParameterlessConstructor()
    {
        var tc = new TestResultContainer();

        _ = AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            static () => _ = new Stub()
        );

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
        Assert.That(fixture.name, Is.EqualTo("Stub.ctor"));
    }

    [Test]
    public void CanBeAppliedOnParameterizedConstructor()
    {
        var tc = new TestResultContainer();

        _ = AllureLifecycle.Instance.RunInContext(
            new AllureContext().WithContainer(tc),
            static () => _ = new Stub(1)
        );

        Assert.That(tc.befores, Has.One.Items);
        var fixture = tc.befores[0];
        Assert.That(fixture.name, Is.EqualTo("Foo"));
        Assert.That(
            fixture.parameters,
            Is.EqualTo([
                new Parameter { name = "foo", value = "1", excluded = false},
            ]).UsingPropertiesComparer()
        );
    }

    [AllureBefore]
    static void VoidMethod() { }

    [AllureBefore]
    static int MethodReturningInt() => default;

    [AllureBefore]
    static async Task AsyncMethod()
    {
        await Task.Delay(1);
    }

    [AllureBefore]
    static async Task<int> AsyncMethodReturningInt()
    {
        await Task.Delay(1);
        return 1;
    }

    class Stub
    {
        [AllureBefore]
        public Stub() { }

        [AllureBefore("Foo")]
        public Stub(int foo) { }
    }

    [AllureBefore("Foo")]
    static void NamedFixture() { }

    [AllureBefore]
    static void FixtureWithParameters(int foo, string bar) { }

    [AllureBefore("Foo {foo}")]
    static void Interpolation(string foo) { }

    [AllureBefore]
    static void IgnoredParameter([AllureParameter(Ignore = true)] string foo) { }

    [AllureBefore]
    static void RenamedParameter([AllureParameter(Name = "bar")] string foo) { }

    [AllureBefore("{foo}")]
    static void InterpolatedRenamedParameter([AllureParameter(Name = "bar")] string foo) { }

    [AllureBefore]
    static void MaskedParameter([AllureParameter(Mode = ParameterMode.Masked)] string foo) { }

    [AllureBefore]
    static void HiddenParameter([AllureParameter(Mode = ParameterMode.Hidden)] string foo) { }
}