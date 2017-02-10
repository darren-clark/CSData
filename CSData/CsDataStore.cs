using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSData
{
    public class CsDataStore
    {
        private readonly ICsDataAdapter defaultAdapter;

        public CsDataStore(ICsDataAdapter defaultAdapter)
        {
            this.defaultAdapter = defaultAdapter;
        }

        public ICsDataResource<TKey, TResource> DefineResource<TKey,TResource>(string name, Func<TResource,TKey> indexer)
        {
            return new CsDataResource<TKey,TResource>(this.defaultAdapter, name, indexer);
        }
    }
}
