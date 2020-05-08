using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Allure.Commons.Storage
{
    internal class AllureStorage
    {
#if !(NET45)
        private readonly AsyncLocal<LinkedList<string>> stepContextLocal = new AsyncLocal<LinkedList<string>>();

        private LinkedList<string> stepContext
        {
            get => stepContextLocal.Value ?? (stepContextLocal.Value = new LinkedList<string>());
            set => stepContextLocal.Value = value;
        }
#else
        // May throw errors when await is using
        private readonly ThreadLocal<LinkedList<string>> stepContextLocal =
            new ThreadLocal<LinkedList<string>>(() => new LinkedList<string>());

        private LinkedList<string> stepContext => stepContextLocal.Value;
#endif
        private readonly ConcurrentDictionary<string, object> storage = new ConcurrentDictionary<string, object>();

        public T Get<T>(string uuid)
        {
            return (T) storage[uuid];
        }

        public T Put<T>(string uuid, T item)
        {
            return (T) storage.GetOrAdd(uuid, item);
        }

        public T Remove<T>(string uuid)
        {
            storage.TryRemove(uuid, out var value);
            return (T) value;
        }

        public void ClearStepContext()
        {
            stepContext.Clear();
        }

        public void StartStep(string uuid)
        {
            stepContext.AddFirst(uuid);
        }

        public void StopStep()
        {
            stepContext.RemoveFirst();
        }

        public string GetRootStep()
        {
            return stepContext.Last?.Value;
        }

        public string GetCurrentStep()
        {
            return stepContext.First?.Value;
        }

        public void AddStep(string parentUuid, string uuid, StepResult stepResult)
        {
            Put(uuid, stepResult);
            Get<ExecutableItem>(parentUuid).steps.Add(stepResult);
        }
    }
}