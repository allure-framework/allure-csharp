using System;
using AspectInjector.Broker;
using NUnit.Allure.Core.Steps;

namespace NUnit.Allure.Attributes
{
    [Injection(typeof(AllureStepAspect))]
    [AttributeUsage(AttributeTargets.Method)]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AllureStepAttribute : Attribute
    {
        public AllureStepAttribute(string name = "")
        {
            StepName = name;
        }

        public string StepName { get; }
    }
}