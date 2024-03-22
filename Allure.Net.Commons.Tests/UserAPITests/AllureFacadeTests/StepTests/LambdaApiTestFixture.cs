using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests.StepTests;

class LambdaApiTestFixture : AllureApiTestFixture
{
    protected interface IErrorInterface { }
    protected class FailException : Exception, IErrorInterface
    {
        public FailException() : base("message") { }
    }
    protected class InheritedFailException : FailException { }

    protected static readonly Action failAction = () => throw new FailException();
    protected static readonly Func<int> failFunc = () => throw new FailException();
    protected static readonly Func<Task> asyncFailAction
        = async () => await Task.FromException(new FailException());
    protected static readonly Func<Task<int>> asyncFailFunc
        = async () => await Task.FromException<int>(new FailException());

    protected static readonly Action breakAction = () => throw new Exception("message");
    protected static readonly Func<int> breakFunc = () => throw new Exception("message");
    protected static readonly Func<Task> asyncBreakAction
        = async () => await Task.FromException(new Exception("message"));
    protected static readonly Func<Task<int>> asyncBreakFunc
        = async () => await Task.FromException<int>(new Exception("message"));

    [SetUp]
    public void SetExceptionTypes()
    {
        this.lifecycle.AllureConfiguration.FailExceptions = new()
        {
            typeof(FailException).FullName
        };
    }
}
