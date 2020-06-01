using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace GraphQLCSharpExample.Loader.Common
{
    public abstract class AbstractListLoader<TKey, TValue> : 
        DataLoaderBase<TKey, IReadOnlyList<TValue>> 
        where TKey: notnull
        where TValue: class
    {
        protected override Task<IReadOnlyList<Result<IReadOnlyList<TValue>>>> FetchAsync(
            IReadOnlyList<TKey> keys, 
            CancellationToken cancellationToken)
        {
            IList<TValue> originalValues = BatchLoad(keys);
            IDictionary<TKey, List<TValue>> groupMap = new Dictionary<TKey, List<TValue>>();
            foreach (TValue? value in originalValues)
            {
                if (value != null)
                {
                    TKey key = GetKey(value);
                    if (key != null)
                    {
                        List<TValue>? groupList;
                        if (!groupMap.TryGetValue(key, out groupList))
                        {
                            groupList = new List<TValue>();
                            groupMap.Add(key, groupList);
                        }
                        groupList.Add(value);
                    }
                }
            }
            Func<IReadOnlyList<Result<IReadOnlyList<TValue>>>> asyncBody =
                () =>
                {
                    var query =
                        from key in keys
                        select Result<IReadOnlyList<TValue>>.Resolve(groupList(groupMap, key));
                    return query.ToList();
                };
            return Task.Run(asyncBody);
        }

        protected abstract TKey GetKey(TValue value);

        protected abstract IList<TValue> BatchLoad(IReadOnlyCollection<TKey> keys);

        private IReadOnlyList<TValue> groupList(IDictionary<TKey, List<TValue>> groupMap, TKey key)
        {
            List<TValue>? groupList;
            if (groupMap.TryGetValue(key, out groupList))
            {
                return groupList;
            }
            return new List<TValue>();
        }
    }
}
