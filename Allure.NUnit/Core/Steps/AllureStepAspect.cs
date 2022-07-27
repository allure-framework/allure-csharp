using System;
using System.Reflection;
using Allure.Net.Commons;
using AspectInjector.Broker;
using NUnit.Allure.Attributes;

namespace NUnit.Allure.Core.Steps
{
    [Aspect(Scope.Global)]
    public class AllureStepAspect
    {
        [Advice(Kind.Around, Targets = Target.Method)]
        public object WrapStep(
            [Argument(Source.Name)] string name,
            [Argument(Source.Metadata)] MethodBase methodBase,
            [Argument(Source.Arguments)] object[] arguments,
            [Argument(Source.Target)] Func<object[], object> method)
        {
            var stepName = methodBase.GetCustomAttribute<AllureStepAttribute>().StepName;

            for (var i = 0; i < arguments.Length; i++)
            {
              
                stepName = stepName?.Replace("{" + i + "}", arguments[i]?.ToString() ?? "null");
            }

            var stepResult = string.IsNullOrEmpty(stepName)
                ? new StepResult {name = name, parameters = AllureStepParameterHelper.CreateParameters(arguments)}
                : new StepResult {name = stepName, parameters = AllureStepParameterHelper.CreateParameters(arguments)};

            object result;
            try
            {
                AllureLifecycle.Instance.StartStep(Guid.NewGuid().ToString(), stepResult);
                result = method(arguments);
                AllureLifecycle.Instance.StopStep(step => stepResult.status = Status.passed);
            }
            catch (Exception e)
            {
                AllureLifecycle.Instance.StopStep(step =>
                {
                    step.statusDetails = new StatusDetails
                    {
                        message = e.Message,
                        trace = e.StackTrace
                    };
                    step.status = Status.failed;
                });
                throw;
            }

            return result;
        }
    }
}