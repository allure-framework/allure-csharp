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
            var stepName = metadata.GetCustomAttribute<AllureStepBaseAttribute>().Name ?? name;

            stepName = metadata.GetParameters().Aggregate(stepName,
                (current, parameterInfo) => current?.Replace("{" + parameterInfo.Name + "}",
                    args[parameterInfo.Position]?.ToString() ?? "null"));

            var stepParameters = metadata.GetParameters()
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
                StartFixture(metadata, stepName);
                StartStep(metadata, stepName, stepParameters);

                executionResult = GetStepExecutionResult(returnType, target, args);

                PassStep(metadata);
                PassFixture(metadata);
            }
            catch (Exception e)
            {
                ThrowStep(metadata, e);
                ThrowFixture(metadata, e);
                throw;
            }

            return executionResult;
        }

        private static void StartStep(MethodBase metadata, string stepName, List<Parameter> stepParameters)
        {
            if (metadata.GetCustomAttribute<AllureStepAttribute>() != null)
            {
                Steps.StartStep(stepName, step => step.parameters = stepParameters);
            }
        }
        
        private static void PassStep(MethodBase metadata)
        {
            if (metadata.GetCustomAttribute<AllureStepAttribute>() != null)
            {
                Steps.PassStep();
            }
        }
        
        private static void ThrowStep(MethodBase metadata, Exception e)
        {
            if (metadata.GetCustomAttribute<AllureStepAttribute>() != null)
            {
                var exceptionStatusDetails = new StatusDetails
                {
                    message = e.Message,
                    trace = e.StackTrace
                };
                
                if (e is XunitException)
                {
                    Steps.FailStep(result => result.statusDetails = exceptionStatusDetails);
                }
                else
                {
                    Steps.BrokeStep(result => result.statusDetails = exceptionStatusDetails);
                }
            }
        }
        
        private static void StartFixture(MethodBase metadata, string stepName)
        {
            if (metadata.GetCustomAttribute<AllureBeforeAttribute>() != null)
            {
                Steps.StartBeforeFixture(stepName);
            }

            if (metadata.GetCustomAttribute<AllureAfterAttribute>() != null)
            {
                Steps.StartAfterFixture(stepName);
            }
        }
        
        private static void PassFixture(MethodBase metadata)
        {
            if (metadata.GetCustomAttribute<AllureBeforeAttribute>() != null ||
                metadata.GetCustomAttribute<AllureAfterAttribute>() != null)
            {
                if (metadata.Name == "InitializeAsync")
                {
                    Steps.StopFixtureSuppressTestCase(result => result.status = Status.passed);
                }
                else
                {
                    Steps.StopFixture(result => result.status = Status.passed);
                }
            }
        }
        
        private static void ThrowFixture(MethodBase metadata, Exception e)
        {
            if (metadata.GetCustomAttribute<AllureBeforeAttribute>() != null ||
                metadata.GetCustomAttribute<AllureAfterAttribute>() != null)
            {
                var exceptionStatusDetails = new StatusDetails
                {
                    message = e.Message,
                    trace = e.StackTrace
                };

                if (metadata.Name == "InitializeAsync")
                {
                    Steps.StopFixtureSuppressTestCase(result =>
                    {
                        result.status = e is XunitException ? Status.failed : Status.broken;
                        result.statusDetails = exceptionStatusDetails;
                    });
                }
                else
                {
                    Steps.StopFixture(result =>
                    {
                        result.status = e is XunitException ? Status.failed : Status.broken;
                        result.statusDetails = exceptionStatusDetails;
                    });
                }
            }
        }

        private object GetStepExecutionResult(Type returnType, Func<object[], object> target, object[] args)
        {
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                var syncResultType = returnType.IsConstructedGenericType
                    ? returnType.GenericTypeArguments[0]
                    : typeof(object);
                return AsyncHandler.MakeGenericMethod(syncResultType)
                    .Invoke(this, new object[] { target, args });
            }

            if (typeof(void).IsAssignableFrom(returnType))
            {
                return target(args);
            }

            return SyncHandler.MakeGenericMethod(returnType)
                .Invoke(this, new object[] { target, args });
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