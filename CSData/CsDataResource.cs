namespace CSData
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal class CsDataResource<TKey,TResource>: ICsDataResource<TKey,TResource>
    {
        private readonly ICsDataAdapter adapter;
        private readonly string name;
        private readonly Func<TResource, TKey> indexer;
        private readonly ConcurrentDictionary<TKey, TResource> resources = new ConcurrentDictionary<TKey, TResource>();
        private readonly Dictionary<TKey, Task<TResource>> inFlight = new Dictionary<TKey, Task<TResource>>();
        private readonly object inFlightLock = new object();

        public CsDataResource(ICsDataAdapter adapter, string name, Func<TResource, TKey> indexer)
        {
            this.adapter = adapter;
            this.name = name;
            this.indexer = indexer;
        }

        private void Inject(IEnumerable<TResource> resources, CsDataInjectOptions options)
        {
            foreach (var resource in resources)
            {
                this.resources[indexer(resource)] = resource;
            }
        }

        void ICsDataResource<TKey, TResource>.Inject(CsDataInjectOptions options, params TResource[] resources)
        {
            this.Inject(resources, options ?? CsDataInjectOptions.Default);
        }

        void ICsDataResource<TKey,TResource>.Inject(params TResource[] resources)
        {
            this.Inject(resources, CsDataInjectOptions.Default);
        }

        private Task<TResource> Find(TKey key, CsDataFindOptions options)
        {
            TResource cached;
            if (resources.TryGetValue(key, out cached))
            {
                return Task.FromResult(cached);
            }

            lock (this.inFlightLock)
            {
                Task<TResource> inFlight;
                if (!this.inFlight.TryGetValue(key, out inFlight))
                {
                    this.inFlight[key] = inFlight = this.adapter.Find<TKey, TResource>(key);
                    inFlight.ContinueWith(t =>
                    {
                        if (t.IsCompleted)
                        {
                            this.Inject(new[] { t.Result }, options);
                        }
                        lock (this.inFlightLock)
                        {
                            this.inFlight.Remove(key);
                        }
                    });
                }

                return inFlight;
            }
        }

        Task<TResource> ICsDataResource<TKey, TResource>.Find(TKey key, CsDataFindOptions options)
        {
            return this.Find(key, options ?? CsDataFindOptions.Default);
        }

        Task<IEnumerable<TResource>> ICsDataResource<TKey, TResource>.FindAll<TQuery>(TQuery query, CsDataFindOptions options)
        {
            throw new NotImplementedException();
        }

        TResource ICsDataResource<TKey, TResource>.Get(TKey key)
        {
            throw new NotImplementedException();
        }

        IEnumerable<TResource> ICsDataResource<TKey, TResource>.Clear()
        {
            this.readerWriterLock.EnterWriteLock();
            try
            {
                var values = resources.Values.Where(t => t.IsCompleted).Select(t => t.Result).ToArray();
                resources.Clear();
                return values;
            }
            finally
            {
                this.readerWriterLock.ExitWriteLock();
            }
        }

        Task<TKey> ICsDataResource<TKey, TResource>.Destroy(TKey key)
        {
            throw new NotImplementedException();
        }

        Task ICsDataResource<TKey, TResource>.DestroyAll()
        {
            throw new NotImplementedException();
        }

        TResource ICsDataResource<TKey, TResource>.Eject(TKey key)
        {
            throw new NotImplementedException();
        }

        IEnumerable<TResource> ICsDataResource<TKey, TResource>.EjectAll(Expression<Func<TResource, bool>> filter)
        {
            throw new NotImplementedException();
        }

        IEnumerable<TResource> ICsDataResource<TKey, TResource>.Filter(Expression<Func<TResource, bool>> filter)
        {
            throw new NotImplementedException();
        }

        IEnumerable<TResource> ICsDataResource<TKey, TResource>.GetAll(IEnumerable<TKey> keys)
        {
            throw new NotImplementedException();
        }

        Task<TResource> ICsDataResource<TKey, TResource>.Save(TKey key)
        {
            throw new NotImplementedException();
        }
    }
}