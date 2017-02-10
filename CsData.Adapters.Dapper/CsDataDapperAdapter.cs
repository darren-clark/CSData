
namespace CsData.Adapters.Dapper
{
    using CSData;
    using global::Dapper;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    public class CsDataDapperAdapter : ICsDataAdapter
    {
        private Func<IDbConnection> connectionFactory;
        private ConcurrentDictionary<Type, string> querySqlCache = new ConcurrentDictionary<Type, string>();

        private static string GetWhereClauseForType(Type type)
        {
            return string.Join(" and ", type.GetProperties().Select(p => string.Format("{0} = @{0}", p.Name)));
        }

        public CsDataDapperAdapter(Func<IDbConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        async Task<TResource> ICsDataAdapter.Find<TKey, TResource>(TKey key)
        {
            return (await this.FindAll<TResource,TKey>(key)).SingleOrDefault();
        }

        public Task<IEnumerable<TResource>> FindAll<TResource, TQuery>(TQuery query)
        {
            using (var connection = this.connectionFactory())
            {
                var querySql = querySqlCache.GetOrAdd(typeof(TQuery),GetWhereClauseForType);
                return connection.QueryAsync<TResource>(string.Format("select * from {0} where {1}", typeof(TResource).Name, querySql), query);
            }
        }
    }
}
