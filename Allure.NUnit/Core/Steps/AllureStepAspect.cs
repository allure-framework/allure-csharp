using System;
using System.Linq;
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
            var parameterInfos = methodBase.GetParameters();
            var stepName = methodBase.GetCustomAttribute<AllureStepAttribute>().StepName;

            var typeFormatters = AllureLifecycle.Instance.TypeFormatters;

            for (var i = 0; i < arguments.Length; i++)
            {
                var parameterType = parameterInfos.ElementAtOrDefault(i)?.ParameterType;
                var typeFormatter = parameterType is null || !typeFormatters.ContainsKey(parameterType)
                    ? null
                    : typeFormatters[parameterType];
                var formattedArgument = typeFormatter?.Format(arguments[i]) ?? arguments[i]?.ToString() ?? "null";
                stepName = stepName?.Replace("{" + i + "}", formattedArgument);
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