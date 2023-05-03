using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectInjector.Broker;
using static Allure.Net.Commons.Steps.AllureStepAttributes;

namespace Allure.Net.Commons.Steps
{
    public abstract class AllureAbstractStepAspect
    {
        internal static readonly MethodInfo AsyncHandler =
            typeof(AllureAbstractStepAspect).GetMethod(nameof(WrapAsync), BindingFlags.NonPublic | BindingFlags.Static);

        internal static readonly MethodInfo AsyncGenericHandler =
            typeof(AllureAbstractStepAspect).GetMethod(nameof(WrapAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Static);

        internal static readonly MethodInfo SyncHandler =
            typeof(AllureAbstractStepAspect).GetMethod(nameof(WrapSync), BindingFlags.NonPublic | BindingFlags.Static);

        internal static readonly MethodInfo SyncVoidHandler =
            typeof(AllureAbstractStepAspect).GetMethod(nameof(WrapSyncVoid), BindingFlags.NonPublic | BindingFlags.Static);

        internal static readonly Type TypeVoidTaskResult = Type.GetType("System.Threading.Tasks.VoidTaskResult");
        internal static readonly Type TypeVoid = typeof(void);
        internal static readonly Type TypeTask = typeof(Task);
        
        public static List<Type> ExceptionTypes { get; set; }

        private static string StartStep(MethodBase metadata, string stepName, List<Parameter> stepParameters)
        {
            if (metadata.GetCustomAttribute<AbstractStepAttribute>() != null)
            {
                return CoreStepsHelper.StartStep(stepName, step => step.parameters = stepParameters);
            }

            return null;
        }

        private static void PassStep(string uuid, MethodBase metadata)
        {
            if (metadata.GetCustomAttribute<AbstractStepAttribute>() != null)
            {
                CoreStepsHelper.PassStep(uuid);
            }
        }

        private static void ThrowStep(string uuid, MethodBase metadata, Exception e)
        {
            if (metadata.GetCustomAttribute<AbstractStepAttribute>() != null)
            {
                var exceptionStatusDetails = new StatusDetails
                {
                    message = e.Message,
                    trace = e.StackTrace
                };
                
                if (ExceptionTypes.Any(exceptionType => exceptionType.IsInstanceOfType(e)))
                {
                    CoreStepsHelper.FailStep(uuid, result => result.statusDetails = exceptionStatusDetails);
                    return;
                }
                CoreStepsHelper.BrokeStep(uuid, result => result.statusDetails = exceptionStatusDetails);
            }
        }

        private static void StartFixture(MethodBase metadata, string stepName)
        {
            if (metadata.GetCustomAttribute<AbstractBeforeAttribute>(inherit: true) != null)
            {
                Console.Out.WriteLine("QWAQWA");
                // throw new Exception("BEFORE FIXTURE");
                CoreStepsHelper.StartBeforeFixture(stepName);
            }

            if (metadata.GetCustomAttribute<AbstractAfterAttribute>(inherit: true) != null)
            {
                CoreStepsHelper.StartAfterFixture(stepName);
            }
        }

        private static void PassFixture(MethodBase metadata)
        {
            if (metadata.GetCustomAttribute<AbstractBeforeAttribute>(inherit: true) != null ||
                metadata.GetCustomAttribute<AbstractAfterAttribute>(inherit: true) != null)
            {
                if (metadata.Name == "InitializeAsync")
                {
                    CoreStepsHelper.StopFixtureSuppressTestCase(result => result.status = Status.passed);
                }
                else
                {
                    CoreStepsHelper.StopFixture(result => result.status = Status.passed);
                }
                
                // TODO: NUnit doing it this way: to be reviewed (!) DO NOT MERGE
                // CoreStepsHelper.StopFixtureSuppressTestCase(result => result.status = Status.passed);
            }
        }

        private static void ThrowFixture(MethodBase metadata, Exception e)
        {
            if (metadata.GetCustomAttribute<AbstractBeforeAttribute>(inherit: true) != null ||
                metadata.GetCustomAttribute<AbstractAfterAttribute>(inherit: true) != null)
            {
                var exceptionStatusDetails = new StatusDetails
                {
                    message = e.Message,
                    trace = e.StackTrace
                };

                if (metadata.Name == "InitializeAsync")
                {
                    CoreStepsHelper.StopFixtureSuppressTestCase(result =>
                    {
                        result.status = ExceptionTypes.Any(exceptionType => exceptionType.IsInstanceOfType(e))
                            ? Status.failed 
                            : Status.broken;
                        result.statusDetails = exceptionStatusDetails;
                    });
                }
                else
                {
                    CoreStepsHelper.StopFixture(result =>
                    {
                        result.status = ExceptionTypes.Any(exceptionType => exceptionType.IsInstanceOfType(e)) 
                            ? Status.failed 
                            : Status.broken;
                        result.statusDetails = exceptionStatusDetails;
                    });
                }
            }
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
    
    [Aspect(Scope.Global)]
    public class AllureStepAspectBase : AllureAbstractStepAspect
    {
        [Advice(Kind.Around, Targets = Target.Method | Target.Constructor)]
        public object Around(
            [Argument(Source.Name)] string name,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Metadata)] MethodBase metadata,
            [Argument(Source.ReturnType)] Type returnType
        )
        {
            var stepNamePattern = metadata.GetCustomAttribute<AbstractStepBaseAttribute>().Name ?? name;
            var stepName = AllureStepParameterHelper.GetStepName(stepNamePattern, metadata, args);
            var stepParameters = AllureStepParameterHelper.GetStepParameters(metadata, args);
            
            if (TypeTask.IsAssignableFrom(returnType))
            {
                if (returnType == TypeTask)
                {
                    return AsyncHandler.Invoke(this, new object[] { target, args, metadata, stepName, stepParameters });
                }

                var syncResultType = returnType.IsConstructedGenericType
                    ? returnType.GenericTypeArguments[0]
                    : TypeVoidTaskResult;
                return AsyncGenericHandler.MakeGenericMethod(syncResultType)
                    .Invoke(this, new object[] { target, args, metadata, stepName, stepParameters });
            }

            if (TypeVoid.IsAssignableFrom(returnType))
            {
                return SyncVoidHandler
                    .Invoke(this, new object[] { target, args, metadata, stepName, stepParameters });
            }

            return SyncHandler.MakeGenericMethod(returnType)
                .Invoke(this, new object[] { target, args, metadata, stepName, stepParameters });
        }
    }
}