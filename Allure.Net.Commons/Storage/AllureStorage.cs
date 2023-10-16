using System.Collections.Concurrent;

#nullable enable

namespace Allure.Net.Commons.Storage
{
    internal class AllureStorage
    {
        readonly ConcurrentDictionary<string, object> storage = new();

        public T Get<T>(string uuid)
        {
            return (T)storage[uuid];
        }

        public T Put<T>(string uuid, T item) where T : notnull
        {
            return (T)storage.GetOrAdd(uuid, item);
        }

        public T Remove<T>(string uuid)
        {
            storage.TryRemove(uuid, out var value);
            return (T)value;
        }
    }
}