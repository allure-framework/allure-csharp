using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserApiTests.NoContextTests.AttributesNoContextTests;

internal class AllureAfterNoContextTests
{
    class AllureAfterAttribute(string name = null) :
        Steps.AllureStepAttributes.AbstractAfterAttribute(name);

    [Test]
    public void AllureBeforeOnActionShouldNotThrow()
    {

        var called = false;

        [AllureAfter]
        void Target()
        {
            called = true;
        }

        Assert.That(Target, Throws.Nothing);
        Assert.That(called, Is.True);
    }

    [Test]
    public void AllureBeforeOnFunctionShouldNotThrow()
    {
        [AllureAfter]
        static int Target() => 1;

        Assert.That(Target, Is.EqualTo(1));
    }

    [Test]
    public void AllureBeforeOnAsyncActionShouldNotThrow()
    {

        var called = false;

        [AllureAfter]
        async Task Target()
        {
            called = true;
            await Task.CompletedTask;
        }

        Assert.That(Target, Throws.Nothing);
        Assert.That(called, Is.True);
    }

    [Test]
    public void AllureBeforeOnAsyncFunctionShouldNotThrow()
    {
        [AllureAfter]
        static async Task<int> Target() => await Task.FromResult(1);

        Assert.That(Target, Is.EqualTo(1));
    }

    public class ArgumentSpy
    {
        public bool called = false;
        public bool Prop => called = true;
    }

    [Test]
    public void AllureBeforeShouldNotCallFormat()
    {
        [AllureAfter("{_}")]
        static int Target(ArgumentSpy _) => default;
        var argSpy = new ArgumentSpy();

        Target(argSpy);

        Assert.That(argSpy.called, Is.False);
    }
}
