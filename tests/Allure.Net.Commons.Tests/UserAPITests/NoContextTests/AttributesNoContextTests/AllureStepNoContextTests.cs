using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserApiTests.NoContextTests.AttributesNoContextTests;

internal class AllureStepNoContextTests
{
    class AllureStepAttribute(string name = null) :
        Steps.AllureStepAttributes.AbstractStepAttribute(name);

    [Test]
    public void AllureStepOnActionShouldNotThrow()
    {

        var called = false;

        [AllureStep]
        void Target()
        {
            called = true;
        }

        Assert.That(Target, Throws.Nothing);
        Assert.That(called, Is.True);
    }

    [Test]
    public void AllureStepOnFunctionShouldNotThrow()
    {
        [AllureStep]
        static int Target() => 1;

        Assert.That(Target, Is.EqualTo(1));
    }

    [Test]
    public void AllureStepOnAsyncActionShouldNotThrow()
    {

        var called = false;

        [AllureStep]
        async Task Target()
        {
            called = true;
            await Task.CompletedTask;
        }

        Assert.That(Target, Throws.Nothing);
        Assert.That(called, Is.True);
    }

    [Test]
    public void AllureStepOnAsyncFunctionShouldNotThrow()
    {
        [AllureStep]
        static async Task<int> Target() => await Task.FromResult(1);

        Assert.That(Target, Is.EqualTo(1));
    }

    public class ArgumentSpy
    {
        public bool called = false;
        public bool Prop => called = true;
    }

    [Test]
    public void AllureStepShouldNotCallFormat()
    {
        [AllureStep("{_}")]
        static int Target(ArgumentSpy _) => default;
        var argSpy = new ArgumentSpy();

        Target(argSpy);

        Assert.That(argSpy.called, Is.False);
    }
}
