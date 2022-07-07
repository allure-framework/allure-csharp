using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Allure.Net.Commons.Storage
{
    internal class AllureStorage
    {
        private readonly ThreadLocal<LinkedList<string>> stepContext = new ThreadLocal<LinkedList<string>>(() =>
        {
            return new LinkedList<string>();
        });

        private readonly ConcurrentDictionary<string, object> storage = new ConcurrentDictionary<string, object>();

        private LinkedList<string> Steps => stepContext.Value;

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
            Steps.Clear();
        }

        public void StartStep(string uuid)
        {
            Steps.AddFirst(uuid);
        }

        public void StopStep()
        {
            Steps.RemoveFirst();
        }

        public string GetRootStep()
        {
            return Steps.Last?.Value;
        }

        public string GetCurrentStep()
        {
            return Steps.First?.Value;
        }

        public void AddStep(string parentUuid, string uuid, StepResult stepResult)
        {
            Put(uuid, stepResult);
            Get<ExecutableItem>(parentUuid).steps.Add(stepResult);
        }
    }
}