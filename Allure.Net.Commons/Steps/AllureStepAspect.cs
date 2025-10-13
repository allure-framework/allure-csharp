using System;
using System.Collections.Generic;
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

        private static void StartStep(MethodBase metadata, string stepName, List<Parameter> stepParameters)
        {
            if (IsStep(metadata))
            {
                ExtendedApi.StartStep(stepName, step => step.parameters = stepParameters);
            }
        }

        private static void PassStep(MethodBase metadata)
        {
            if (IsStep(metadata))
            {
                ExtendedApi.PassStep();
            }
        }

        private static void ThrowStep(MethodBase metadata, Exception e)
        {
            if (IsStep(metadata))
            {
                ExtendedApi.ResolveStep(e);
            }
        }

        private static void StartFixture(MethodBase metadata, string fixtureName)
        {
            if (IsBeforeFixture(metadata))
            {
                ExtendedApi.StartBeforeFixture(fixtureName);
            }

            if (IsAfterFixture(metadata))
            {
                ExtendedApi.StartAfterFixture(fixtureName);
            }
        }

        private static void PassFixture(MethodBase metadata)
        {
            if (IsBeforeFixture(metadata) || IsAfterFixture(metadata))
            {
                ExtendedApi.PassFixture();
            }
        }

        private static void ThrowFixture(MethodBase metadata, Exception e)
        {
            if (IsBeforeFixture(metadata) || IsAfterFixture(metadata))
            {
                ExtendedApi.ResolveFixture(e);
            }
        }

        // ------------------------------

        private static void BeforeTargetInvoke(
            MethodBase metadata,
            Lazy<string> stepName,
            Lazy<List<Parameter>> stepParameters
        )
        {
            if (ExtendedApi.HasContainer)
            {
                StartFixture(metadata, stepName.Value);
            }
            if (ExtendedApi.HasTestOrFixture)
            {
                StartStep(metadata, stepName.Value, stepParameters.Value);
            }
        }

        private static void AfterTargetInvoke(MethodBase metadata)
        {
            PassStep(metadata);
            PassFixture(metadata);
        }

        private static void OnTargetInvokeException(MethodBase metadata, Exception e)
        {
            ThrowStep(metadata, e);
            ThrowFixture(metadata, e);
        }

        // ------------------------------

        private static T WrapSync<T>(
            Func<object[], object> target,
            object[] args,
            MethodBase metadata,
            Lazy<string> stepName,
            Lazy<List<Parameter>> stepParameters
        )
        {
            try
            {
                BeforeTargetInvoke(metadata, stepName, stepParameters);
                var result = (T)target(args);
                AfterTargetInvoke(metadata);

                return result;
            }
            catch (Exception e)
            {
                OnTargetInvokeException(metadata, e);
                throw;
            }
        }

        private static void WrapSyncVoid(
            Func<object[], object> target,
            object[] args,
            MethodBase metadata,
            Lazy<string> stepName,
            Lazy<List<Parameter>> stepParameters
        )
        {
            try
            {
                BeforeTargetInvoke(metadata, stepName, stepParameters);
                target(args);
                AfterTargetInvoke(metadata);
            }
            catch (Exception e)
            {
                OnTargetInvokeException(metadata, e);
                throw;
            }
        }

        private static async Task WrapAsync(
            Func<object[], object> target,
            object[] args,
            MethodBase metadata,
            Lazy<string> stepName,
            Lazy<List<Parameter>> stepParameters
        )
        {
            try
            {
                BeforeTargetInvoke(metadata, stepName, stepParameters);
                await ((Task)target(args)).ConfigureAwait(false);
                AfterTargetInvoke(metadata);
            }
            catch (Exception e)
            {
                OnTargetInvokeException(metadata, e);
                throw;
            }
        }

        private static async Task<T> WrapAsyncGeneric<T>(
            Func<object[], object> target,
            object[] args,
            MethodBase metadata,
            Lazy<string> stepName,
            Lazy<List<Parameter>> stepParameters
        )
        {
            try
            {
                BeforeTargetInvoke(metadata, stepName, stepParameters);
                var result = await ((Task<T>)target(args)).ConfigureAwait(false);
                AfterTargetInvoke(metadata);

                return result;
            }
            catch (Exception e)
            {
                OnTargetInvokeException(metadata, e);
                throw;
            }
        }

        static bool IsBeforeFixture(MethodBase method) =>
            Attribute.IsDefined(method, typeof(AbstractBeforeAttribute), true);

        static bool IsAfterFixture(MethodBase method) =>
            Attribute.IsDefined(method, typeof(AbstractAfterAttribute), true);

        static bool IsStep(MethodBase method) =>
            Attribute.IsDefined(method, typeof(AbstractStepAttribute), true);
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
            var formatters = AllureLifecycle.Instance.TypeFormatters;
            var stepNamePattern = metadata.GetCustomAttribute<AbstractStepBaseAttribute>().Name ?? name;
            var stepName = new Lazy<string>(
                () => AllureStepParameterHelper.GetStepName(
                    stepNamePattern,
                    metadata,
                    args,
                    formatters
                )
            );
            var stepParameters = new Lazy<List<Parameter>>(
                () => AllureStepParameterHelper.GetStepParameters(
                    metadata,
                    args,
                    formatters
                )
            );

            if (TypeTask.IsAssignableFrom(returnType))
            {
                if (returnType == TypeTask)
                {
                    return AsyncHandler.Invoke(this, [target, args, metadata, stepName, stepParameters]);
                }

                var syncResultType = returnType.IsConstructedGenericType
                    ? returnType.GenericTypeArguments[0]
                    : TypeVoidTaskResult;
                return AsyncGenericHandler.MakeGenericMethod(syncResultType)
                    .Invoke(this, [target, args, metadata, stepName, stepParameters]);
            }

            if (TypeVoid.IsAssignableFrom(returnType))
            {
                return SyncVoidHandler
                    .Invoke(this, [target, args, metadata, stepName, stepParameters]);
            }

            return SyncHandler.MakeGenericMethod(returnType)
                .Invoke(this, [target, args, metadata, stepName, stepParameters]);
        }
    }
}