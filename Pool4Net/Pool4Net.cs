using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Pool4Net
{
    public class Pool4Net<T>
    {
        private const string DEFAULT_GROUP = "Default";

        private ConcurrentDictionary<string, ConcurrentBag<T>> storage;
        private Func<T> generator;
        private Action<T> cleaner;

        public Pool4Net(Func<T> generator, Action<T> cleaner) : this(generator, cleaner, new[] { DEFAULT_GROUP })
        {
        }

        public Pool4Net(Func<T> generator, Action<T> cleaner, IEnumerable<string> groups)
        {
            this.generator = generator ?? throw new ArgumentException("The generator cannot be null");
            this.cleaner = cleaner;

            storage = new ConcurrentDictionary<string, ConcurrentBag<T>>();
            foreach (string group in groups) storage.GetOrAdd(group, new ConcurrentBag<T>());
        }

        public T Get()
        {
            T result;
            if (storage[DEFAULT_GROUP].TryTake(out result)) return result;
            return generator();
        }

        public T Get(string group)
        {
            T result;
            if (storage[group].TryTake(out result)) return result;
            return generator();
        }

        public void Release(T item)
        {
            cleaner?.Invoke(item);
            storage[DEFAULT_GROUP].Add(item);
        }

        public void Release(string group, T item)
        {
            cleaner?.Invoke(item);
            storage[group].Add(item);
        }
    }
}