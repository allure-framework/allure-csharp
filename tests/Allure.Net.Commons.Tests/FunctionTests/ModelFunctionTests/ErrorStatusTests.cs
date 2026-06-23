using System;
using Allure.Net.Commons.Functions;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.FunctionTests.ModelFunctionTests;

class ErrorStatusTests
{
    [Test]
    public void ShouldReturnFalseIfKnownErrorsIsNull()
    {
        Assert.That(
            ModelFunctions.IsKnownError(null, new InvalidOperationException()),
            Is.False
        );
    }

    [Test]
    public void ShouldReturnFalseIfKnownErrorsIsEmpty()
    {
        Assert.That(
            ModelFunctions.IsKnownError([], new InvalidOperationException()),
            Is.False
        );
    }

    [Test]
    public void ShouldReturnTrueForExactExceptionTypeMatch()
    {
        Assert.That(
            ModelFunctions.IsKnownError(
                [typeof(InvalidOperationException).FullName],
                new InvalidOperationException()
            ),
            Is.True
        );
    }

    [Test]
    public void ShouldReturnTrueForBaseExceptionTypeMatch()
    {
        Assert.That(
            ModelFunctions.IsKnownError(
                [typeof(Exception).FullName],
                new InvalidOperationException()
            ),
            Is.True
        );
    }

    [Test]
    public void ShouldReturnTrueForIntermediateBaseTypeMatch()
    {
        Assert.That(
            ModelFunctions.IsKnownError(
                [typeof(BaseTestException).FullName],
                new DerivedTestException()
            ),
            Is.True
        );
    }

    [Test]
    public void ShouldReturnTrueForImplementedInterfaceMatch()
    {
        Assert.That(
            ModelFunctions.IsKnownError(
                [typeof(ITestErrorMarker).FullName],
                new InterfaceTestException()
            ),
            Is.True
        );
    }

    [Test]
    public void ShouldReturnFalseIfNoTypeInChainMatches()
    {
        Assert.That(
            ModelFunctions.IsKnownError(
                [typeof(ArgumentException).FullName],
                new InvalidOperationException()
            ),
            Is.False
        );
    }

    [Test]
    public void ShouldUseFullTypeNameComparison()
    {
        Assert.That(
            ModelFunctions.IsKnownError(
                [nameof(InvalidOperationException)],
                new InvalidOperationException()
            ),
            Is.False
        );
    }

    [Test]
    public void ShouldBeCaseSensitive()
    {
        Assert.That(
            ModelFunctions.IsKnownError(
                [typeof(InvalidOperationException).FullName.ToLowerInvariant()],
                new InvalidOperationException()
            ),
            Is.False
        );
    }

    [Test]
    public void ShouldReturnFailedForExactExceptionTypeMatch()
    {
        Assert.That(
            ModelFunctions.ResolveErrorStatus(
                [typeof(InvalidOperationException).FullName],
                new InvalidOperationException()
            ),
            Is.EqualTo(Status.failed)
        );
    }

    [Test]
    public void ShouldReturnFailedForBaseExceptionTypeMatch()
    {
        Assert.That(
            ModelFunctions.ResolveErrorStatus(
                [typeof(Exception).FullName],
                new InvalidOperationException()
            ),
            Is.EqualTo(Status.failed)
        );
    }

    [Test]
    public void ShouldReturnFailedForInterfaceMatch()
    {
        Assert.That(
            ModelFunctions.ResolveErrorStatus(
                [typeof(ITestErrorMarker).FullName],
                new InterfaceTestException()
            ),
            Is.EqualTo(Status.failed)
        );
    }

    [Test]
    public void ShouldReturnBrokenIfNoMatchFound()
    {
        Assert.That(
            ModelFunctions.ResolveErrorStatus(
                [typeof(ArgumentException).FullName],
                new InvalidOperationException()
            ),
            Is.EqualTo(Status.broken)
        );
    }

    [Test]
    public void ShouldReturnBrokenIfFailExceptionsIsNull()
    {
        Assert.That(
            ModelFunctions.ResolveErrorStatus(null, new InvalidOperationException()),
            Is.EqualTo(Status.broken)
        );
    }

    [Test]
    public void ShouldReturnBrokenIfFailExceptionsIsEmpty()
    {
        Assert.That(
            ModelFunctions.ResolveErrorStatus([], new InvalidOperationException()),
            Is.EqualTo(Status.broken)
        );
    }

    [Test]
    public void ShouldReturnNullIfExceptionIsNull()
    {
        Assert.That(ModelFunctions.ToStatusDetails(null), Is.Null);
    }

    [Test]
    public void ShouldUseExceptionMessageWhenNotEmpty()
    {
        var details = ModelFunctions.ToStatusDetails(
            new InvalidOperationException("boom")
        );

        Assert.That(details.message, Is.EqualTo("boom"));
    }

    [Test]
    public void ShouldUseExceptionTypeNameWhenMessageIsEmpty()
    {
        var details = ModelFunctions.ToStatusDetails(new EmptyMessageException());

        Assert.That(details.message, Is.EqualTo(nameof(EmptyMessageException)));
    }

    [Test]
    public void ShouldUseExceptionTypeNameWhenMessageIsNull()
    {
        var details = ModelFunctions.ToStatusDetails(new NullMessageException());

        Assert.That(details.message, Is.EqualTo(nameof(NullMessageException)));
    }

    [Test]
    public void ShouldSetTraceToExceptionToString()
    {
        var error = new InvalidOperationException("boom");
        var details = ModelFunctions.ToStatusDetails(error);

        Assert.That(details.trace, Is.EqualTo(error.ToString()));
    }

    [Test]
    public void ShouldPreserveInnerExceptionDetailsInTrace()
    {
        var error = new InvalidOperationException(
            "outer",
            new ArgumentException("inner")
        );
        var details = ModelFunctions.ToStatusDetails(error);

        Assert.That(details.trace, Does.Contain("outer"));
        Assert.That(details.trace, Does.Contain("inner"));
    }

    interface ITestErrorMarker;

    class BaseTestException : Exception;

    class DerivedTestException : BaseTestException;

    class InterfaceTestException : Exception, ITestErrorMarker;

    class EmptyMessageException : Exception
    {
        public override string Message => "";
    }

    class NullMessageException : Exception
    {
        public override string Message => null;
    }
}
