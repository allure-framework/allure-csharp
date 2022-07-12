using AspectInjector.Broker;
using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Net.Commons;
using Xunit.Sdk;
using System.Reflection;
using System.Threading.Tasks;
using Allure.Xunit;
using Allure.XUnit.Attributes.Steps;

namespace Allure.XUnit
{
    [Aspect(Scope.Global)]
    public class AllureStepAspect
    {
        private static readonly MethodInfo AsyncHandler =
            typeof(AllureStepAspect).GetMethod(nameof(WrapAsync), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo SyncHandler =
            typeof(AllureStepAspect).GetMethod(nameof(WrapSync), BindingFlags.NonPublic | BindingFlags.Static);

        [Advice(Kind.Around)]
        public object Around([Argument(Source.Name)] string name,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Metadata)] MethodBase metadata,
            [Argument(Source.ReturnType)] Type returnType)
        {
            object executionResult;

            var allureBeforeAttribute = metadata.GetCustomAttribute<AllureBeforeAttribute>();
            var allureAfterAttribute = metadata.GetCustomAttribute<AllureAfterAttribute>();
            var stepName = metadata.GetCustomAttribute<AllureStepBaseAttribute>().Name ?? name;

            foreach (var parameterInfo in metadata.GetParameters())
            {
                stepName = stepName?.Replace("{" + parameterInfo.Name + "}",
                    args[parameterInfo.Position]?.ToString() ?? "null");
            }

            List<Parameter> stepParameters = metadata.GetParameters()
                .Select(x => (
                    name: x.GetCustomAttribute<NameAttribute>()?.Name ?? x.Name,
                    skip: x.GetCustomAttribute<SkipAttribute>() != null))
                .Zip(args, (parameter, value) => parameter.skip
                    ? null
                    : new Parameter
                    {
                        name = parameter.name,
                        value = value?.ToString()
                    })
                .Where(x => x != null)
                .ToList();

            try
            {
                if (allureBeforeAttribute == null && allureAfterAttribute == null)
                {
                    Steps.StartStep(stepName, step => step.parameters = stepParameters);
                }
                
                if (allureBeforeAttribute != null)
                {
                    Steps.StartBeforeFixture(allureBeforeAttribute.Name ?? name);
                }

                if (allureAfterAttribute != null)
                {
                    Steps.StartAfterFixture(allureAfterAttribute.Name ?? name);
                }

                if (typeof(Task).IsAssignableFrom(returnType))
                {
                    var syncResultType = returnType.IsConstructedGenericType
                        ? returnType.GenericTypeArguments[0]
                        : typeof(object);
                    executionResult = AsyncHandler.MakeGenericMethod(syncResultType)
                        .Invoke(this, new object[] { target, args });
                }
                else if (typeof(void).IsAssignableFrom(returnType))
                {
                    executionResult = target(args);
                }
                else
                {
                    executionResult = SyncHandler.MakeGenericMethod(returnType)
                        .Invoke(this, new object[] { target, args });
                }

                if (allureBeforeAttribute == null && allureAfterAttribute == null)
                {
                    Steps.PassStep();
                }
                else
                {
                    if (metadata.Name == "InitializeAsync")
                    {
                        // Workaround for IAsyncLifetime. Don't use it.
                        Steps.StopFixtureSuppressTestCase(result => result.status = Status.passed);
                    }
                    else
                    {
                        Steps.StopFixture(result => result.status = Status.passed);
                    }
                }
            }
            catch (Exception e)
            {
                var exceptionStatusDetails = new StatusDetails
                {
                    message = e.Message,
                    trace = e.StackTrace
                };

                if (allureBeforeAttribute == null && allureAfterAttribute == null)
                {
                    if (e is XunitException)
                    {
                        Steps.FailStep(result => result.statusDetails = exceptionStatusDetails);
                    }
                    else
                    {
                        Steps.BrokeStep(result => result.statusDetails = exceptionStatusDetails);
                    }
                }
                else
                {
                    if (metadata.Name == "InitializeAsync")
                    {
                        // Workaround for IAsyncLifetime. Don't use it.
                        Steps.StopFixtureSuppressTestCase(result => result.status = Status.failed);
                    }
                    else
                    {
                        Steps.StopFixture(result => result.status = Status.failed);
                    }
                }

                throw;
            }

            return executionResult;
        }

        private static T WrapSync<T>(Func<object[], object> target, object[] args)
        {
            try
            {
                return (T)target(args);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        private static async Task<T> WrapAsync<T>(Func<object[], object> target, object[] args)
        {
            try
            {
                return await (Task<T>)target(args);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
    }
}