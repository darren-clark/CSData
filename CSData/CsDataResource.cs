namespace CSData
{
    using System;
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
        private readonly Dictionary<TKey, Task<TResource>> resources = new Dictionary<TKey, Task<TResource>>();
        private readonly Dictionary<Expression<Func<TResource, bool>>, Task<TResource>> filters = new Dictionary<Expression<Func<TResource, bool>>, Task<TResource>>();
        private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

        public CsDataResource(ICsDataAdapter adapter, string name, Func<TResource, TKey> indexer)
        {
            this.adapter = adapter;
            this.name = name;
            this.indexer = indexer;
        }

        private void Inject(IEnumerable<TResource> resources, CsDataInjectOptions options)
        {
            readerWriterLock.EnterWriteLock();
            try
            {
                foreach (var resource in resources)
                {
                    this.resources[indexer(resource)] = Task.FromResult(resource);
                }
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
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
            Task<TResource> result;
            readerWriterLock.EnterReadLock();
            try
            {
                if (this.resources.TryGetValue(key, out result))
                    return result;
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }

            readerWriterLock.EnterUpgradeableReadLock();
            try
            {
                if (this.resources.TryGetValue(key, out result))
                    return result;

                readerWriterLock.EnterWriteLock();
                try
                {
                    this.resources[key] = result = this.adapter.Find<TKey, TResource>(key);
                    result.ContinueWith(t =>
                    {
                        this.readerWriterLock.EnterWriteLock();
                        try
                        {
                            this.resources.Remove(key);
                        }
                        finally
                        {
                            this.readerWriterLock.ExitWriteLock();
                        }
                    }, TaskContinuationOptions.NotOnRanToCompletion);
                }
                finally
                {
                    readerWriterLock.ExitWriteLock();
                }

                return result;
            }
            finally
            {
                readerWriterLock.ExitUpgradeableReadLock();
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