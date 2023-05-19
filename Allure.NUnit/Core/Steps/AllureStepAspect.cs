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

        private static readonly MethodInfo AsyncGenericHandler =
            typeof(AllureStepAspect).GetMethod(nameof(WrapAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo SyncHandler =
            typeof(AllureStepAspect).GetMethod(nameof(WrapSync), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly MethodInfo SyncVoidHandler =
            typeof(AllureStepAspect).GetMethod(nameof(WrapSyncVoid), BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly Type _typeVoidTaskResult = Type.GetType("System.Threading.Tasks.VoidTaskResult");
        private static readonly Type _typeVoid = typeof(void);
        private static readonly Type _typeTask = typeof(Task);

        [Advice(Kind.Around, Targets = Target.Method)]
        public object Around(
            [Argument(Source.Name)] string name,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Metadata)] MethodBase metadata,
            [Argument(Source.ReturnType)] Type returnType
        )
        {
            var stepNamePattern = metadata.GetCustomAttribute<AllureStepBaseAttribute>().Name ?? name;
            var stepName = AllureStepParameterHelper.GetStepName(stepNamePattern, metadata, args);
            var stepParameters = GetStepParameters(metadata, args);

            if (_typeTask.IsAssignableFrom(returnType))
            {
                if (returnType == _typeTask)
                {
                    return AsyncHandler.Invoke(this, new object[] { target, args, metadata, stepName, stepParameters });
                }

                var syncResultType = returnType.IsConstructedGenericType
                    ? returnType.GenericTypeArguments[0]
                    : _typeVoidTaskResult;
                return AsyncGenericHandler.MakeGenericMethod(syncResultType)
                    .Invoke(this, new object[] { target, args, metadata, stepName, stepParameters });
            }
            else if (_typeVoid.IsAssignableFrom(returnType))
            {
                return SyncVoidHandler
                    .Invoke(this, new object[] { target, args, metadata, stepName, stepParameters });
            }
            else
            {
                try
                {
                    return SyncHandler.MakeGenericMethod(returnType)
                        .Invoke(this, new object[] { target, args, metadata, stepName, stepParameters });
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException ?? e;
                }
            }
        }

        // ------------------------------

        private static string StartStep(MethodBase metadata, string stepName, List<Parameter> stepParameters)
        {
            if (metadata.GetCustomAttribute<AllureStepAttribute>() != null)
            {
                return StepsHelper.StartStep(stepName, step => step.parameters = stepParameters);
            }

            return null;
        }

        private static void PassStep(string uuid, MethodBase metadata)
        {
            if (metadata.GetCustomAttribute<AllureStepAttribute>() != null)
            {
                StepsHelper.PassStep(uuid);
            }
        }

        private static void ThrowStep(string uuid, MethodBase metadata, Exception e)
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
                    StepsHelper.FailStep(uuid, result => result.statusDetails = exceptionStatusDetails);
                }
                else
                {
                    StepsHelper.BrokeStep(uuid, result => result.statusDetails = exceptionStatusDetails);
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

        // ------------------------------

        private List<Parameter> GetStepParameters(MethodBase metadata, object[] args)
        {
            return metadata.GetParameters()
                .Select(x => (
                    name: x.GetCustomAttribute<NameAttribute>()?.Name ?? x.Name,
                    skip: x.GetCustomAttribute<SkipAttribute>() != null))
                .Zip(args,
                    (parameter, value) => parameter.skip
                        ? null
                        : new Parameter
                        {
                            name = parameter.name,
                            value = value?.ToString()
                        })
                .Where(x => x != null)
                .ToList();
        }

        // ------------------------------

        private static string BeforeTargetInvoke(MethodBase metadata, string stepName, List<Parameter> stepParameters)
        {
            StartFixture(metadata, stepName);
            var stepUuid = StartStep(metadata, stepName, stepParameters);
            return stepUuid;
        }

        private static void AfterTargetInvoke(string stepUuid, MethodBase metadata)
        {
            PassStep(stepUuid, metadata);
            PassFixture(metadata);
        }

        private static void OnTargetInvokeException(string stepUuid, MethodBase metadata, Exception e)
        {
            ThrowStep(stepUuid, metadata, e);
            ThrowFixture(metadata, e);
        }

        // ------------------------------

        private static T WrapSync<T>(
            Func<object[], object> target,
            object[] args,
            MethodBase metadata,
            string stepName,
            List<Parameter> stepParameters
        )
        {
            string stepUuid = null;

            try
            {
                stepUuid = BeforeTargetInvoke(metadata, stepName, stepParameters);
                var result = (T)target(args);
                AfterTargetInvoke(stepUuid, metadata);

                return result;
            }
            catch (Exception e)
            {
                OnTargetInvokeException(stepUuid, metadata, e);
                throw;
            }
        }

        private static void WrapSyncVoid(
            Func<object[], object> target,
            object[] args,
            MethodBase metadata,
            string stepName,
            List<Parameter> stepParameters
        )
        {
            string stepUuid = null;

            try
            {
                stepUuid = BeforeTargetInvoke(metadata, stepName, stepParameters);
                target(args);
                AfterTargetInvoke(stepUuid, metadata);
            }
            catch (Exception e)
            {
                OnTargetInvokeException(stepUuid, metadata, e);
                throw;
            }
        }

        private static async Task WrapAsync(
            Func<object[], object> target,
            object[] args,
            MethodBase metadata,
            string stepName,
            List<Parameter> stepParameters
        )
        {
            string stepUuid = null;

            try
            {
                stepUuid = BeforeTargetInvoke(metadata, stepName, stepParameters);
                await ((Task)target(args)).ConfigureAwait(false);
                AfterTargetInvoke(stepUuid, metadata);
            }
            catch (Exception e)
            {
                OnTargetInvokeException(stepUuid, metadata, e);
                throw;
            }
        }

        private static async Task<T> WrapAsyncGeneric<T>(
            Func<object[], object> target,
            object[] args,
            MethodBase metadata,
            string stepName,
            List<Parameter> stepParameters
        )
        {
            string stepUuid = null;

            try
            {
                stepUuid = BeforeTargetInvoke(metadata, stepName, stepParameters);
                var result = await ((Task<T>)target(args)).ConfigureAwait(false);
                AfterTargetInvoke(stepUuid, metadata);

                return result;
            }
            catch (Exception e)
            {
                OnTargetInvokeException(stepUuid, metadata, e);
                throw;
            }
        }
    }
}