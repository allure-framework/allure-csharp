using Allure.Commons;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Allure.Commons.Storage
{
    class AllureStorage
    {
        private ConcurrentDictionary<string, object> storage = new ConcurrentDictionary<string, object>();
        private ThreadLocal<LinkedList<string>> stepContext = new ThreadLocal<LinkedList<string>>(() =>
        {
            return new LinkedList<string>();
        });

        public T Get<T>(string uuid)
        {
            return (T)storage[uuid];
        }
        public T Put<T>(T item)
        {
            dynamic obj = item;
            return (T)storage.AddOrUpdate((string)obj.uuid, item, (key, value) => item);
        }
        public T Put<T>(string uuid, T item)
        {
            return (T)storage.AddOrUpdate(uuid, item, (key, value) => item);
        }
        public T Remove<T>(string uuid)
        {
            storage.TryRemove(uuid, out object value);
            return (T)value;
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
