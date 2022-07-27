using System;
using AspectInjector.Broker;
using NUnit.Allure.Core.Steps;

namespace NUnit.Allure.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    [Injection(typeof(AllureStepAspect))]
    public class AllureBeforeAttribute : AllureStepBaseAttribute
    {
        public AllureBeforeAttribute(string name = null) : base(name)
        {
        }
    }
}