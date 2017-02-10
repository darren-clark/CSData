namespace CSData
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq.Expressions;

    public interface ICsDataResource<TKey, TResource>
    {
        void Inject(CsDataInjectOptions options, params TResource[] resources);
        void Inject(params TResource[] resources);
        Task<TResource> Find(TKey key, CsDataFindOptions options = null);
        Task<IEnumerable<TResource>> FindAll<TQuery>(TQuery query, CsDataFindOptions options = null);

        TResource Get(TKey key);
        IEnumerable<TResource> GetAll(IEnumerable<TKey> keys);
        IEnumerable<TResource> Clear();
        Task<TKey> Destroy(TKey key);
        Task DestroyAll();
        TResource Eject(TKey key);

        IEnumerable<TResource> EjectAll(Expression<Func<TResource, bool>> filter);
        IEnumerable<TResource> Filter(Expression<Func<TResource, bool>> filter);
        Task<TResource> Save(TKey key);
    }
}