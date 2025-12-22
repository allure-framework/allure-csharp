using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserApiTests.NoContextTests.AttributesNoContextTests;

internal class AllureBeforeNoContextTests
{
    class AllureBeforeAttribute(string name = null) :
        Steps.AllureStepAttributes.AbstractBeforeAttribute(name);

    [Test]
    public void AllureBeforeOnActionShouldNotThrow()
    {

        var called = false;

        [AllureBefore]
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
        [AllureBefore]
        static int Target() => 1;

        Assert.That(Target, Is.EqualTo(1));
    }

    [Test]
    public void AllureBeforeOnAsyncActionShouldNotThrow()
    {

        var called = false;

        [AllureBefore]
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
        [AllureBefore]
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
        [AllureBefore("{_}")]
        static int Target(ArgumentSpy _) => default;
        var argSpy = new ArgumentSpy();

        Target(argSpy);

        Assert.That(argSpy.called, Is.False);
    }
}
