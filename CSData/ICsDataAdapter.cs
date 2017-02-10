
namespace CSData
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICsDataAdapter
    {
        Task<TResource> Find<TKey, TResource>(TKey key);
        Task<IEnumerable<TResource>> FindAll<TResource, TQuery>(TQuery query);
    }
}