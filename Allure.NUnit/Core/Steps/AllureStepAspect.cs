using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Allure.Net.Commons;
using AspectInjector.Broker;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.Allure.Core.Steps
{
    [Aspect(Scope.Global)]
    public class AllureStepAspect
    {
        private static readonly MethodInfo AsyncHandler =
            typeof(AllureStepAspect).GetMethod(nameof(WrapAsync), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo SyncHandler =
            typeof(AllureStepAspect).GetMethod(nameof(WrapSync), BindingFlags.NonPublic | BindingFlags.Static);
        
        private static readonly Type _voidTaskResult = Type.GetType("System.Threading.Tasks.VoidTaskResult");

        [Advice(Kind.Around, Targets = Target.Method)]
        public object Around([Argument(Source.Name)] string name,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Metadata)] MethodBase metadata,
            [Argument(Source.ReturnType)] Type returnType)
        {
            object executionResult;
            var stepName = metadata.GetCustomAttribute<AllureStepBaseAttribute>().Name ?? name;

            for (var i = 0; i < args.Length; i++)
            {
                stepName = stepName?.Replace("{" + i + "}", args[i]?.ToString() ?? "null");
                if (stepName.Contains("{" + i + "}"))
                {
                    // TODO: provide error description link
                    Console.Error.WriteLine("Indexed step arguments is obsolete. Use named arguments instead. ({0} -> {argumentName}) See: LINK_TO_ERROR");
                }
            }
            
            stepName = metadata.GetParameters().Aggregate(stepName,
                (current, parameterInfo) => current?.Replace("{" + parameterInfo.Name + "}",
                    args[parameterInfo.Position]?.ToString() ?? "null"));

            var stepResult = string.IsNullOrEmpty(stepName)
                ? new StepResult {name = name, parameters = AllureStepParameterHelper.CreateParameters(args)}
                : new StepResult {name = stepName, parameters = AllureStepParameterHelper.CreateParameters(args)};

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
                if (executionResult != null && typeof(Task).IsAssignableFrom(executionResult.GetType()))
                {
                    ((Task)executionResult).ContinueWith((task) =>
                    {
                        if (task.IsFaulted)
                        {
                            var e = task.Exception;
                            ThrowStep(metadata, e?.InnerException);
                            ThrowFixture(metadata, e?.InnerException);
                        }
                        else
                        {
                            PassStep(metadata);
                            PassFixture(metadata);
                        }
                    });
                }
                else
                {
                    PassStep(metadata);
                    PassFixture(metadata);
                }
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
                StepsHelper.StartStep(stepName, step => step.parameters = stepParameters);
            }
        }
        
        private static void PassStep(MethodBase metadata)
        {
            if (metadata.GetCustomAttribute<AllureStepAttribute>() != null)
            {
                StepsHelper.PassStep();
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
                
                if (e is NUnitException || e is AssertionException)
                {
                    StepsHelper.FailStep(result => result.statusDetails = exceptionStatusDetails);
                }
                else
                {
                    StepsHelper.BrokeStep(result => result.statusDetails = exceptionStatusDetails);
                }
            }
        }
        
        private static void StartFixture(MethodBase metadata, string stepName)
        {
            if (metadata.GetCustomAttribute<AllureBeforeAttribute>() != null)
            {
                StepsHelper.StartBeforeFixture(stepName);
            }

            if (metadata.GetCustomAttribute<AllureAfterAttribute>() != null)
            {
                StepsHelper.StartAfterFixture(stepName);
            }
        }
        
        private static void PassFixture(MethodBase metadata)
        {
            if (metadata.GetCustomAttribute<AllureBeforeAttribute>() != null ||
                metadata.GetCustomAttribute<AllureAfterAttribute>() != null)
            {
                StepsHelper.StopFixtureSuppressTestCase(result => result.status = Status.passed);
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
                    StepsHelper.StopFixtureSuppressTestCase(result =>
                    {
                        result.status = e is NUnitException ? Status.failed : Status.broken;
                        result.statusDetails = exceptionStatusDetails;
                    });
                }
                else
                {
                    StepsHelper.StopFixture(result =>
                    {
                        result.status = e is NUnitException ? Status.failed : Status.broken;
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
                    : _voidTaskResult;
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
            return (T)target(args);
        }

        private static async Task<T> WrapAsync<T>(Func<object[], object> target, object[] args)
        {
            return await (Task<T>)target(args);
        }
    }
}