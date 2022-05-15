using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Allure.Commons.Storage
{
    internal class AllureStorage
    {
        private readonly AsyncLocal<LinkedList<string>> stepContext = new AsyncLocal<LinkedList<string>>(() =>
        {
            return new LinkedList<string>();
        });

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
            stepContext.Value.Clear();
        }

        public void StartStep(string uuid)
        {
            stepContext.Value.AddFirst(uuid);
        }

        public void StopStep()
        {
            stepContext.Value.RemoveFirst();
        }

        public string GetRootStep()
        {
            return stepContext.Value.Last?.Value;
        }

        public string GetCurrentStep()
        {
            return stepContext.Value.First?.Value;
        }

        public void AddStep(string parentUuid, string uuid, StepResult stepResult)
        {
            Put(uuid, stepResult);
            Get<ExecutableItem>(parentUuid).steps.Add(stepResult);
        }
    }
}
