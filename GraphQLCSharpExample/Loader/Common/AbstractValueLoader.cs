using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace GraphQLCSharpExample.Loader.Common
{
    public abstract class AbstractValueLoader<TKey, TValue> :
        DataLoaderBase<TKey, TValue>
        where TKey : notnull
        where TValue : class
    {
        protected override Task<IReadOnlyList<Result<TValue>>> FetchAsync(
            IReadOnlyList<TKey> keys,
            CancellationToken cancellationToken)
        {
            IList<TValue> originalValues = BatchLoad(keys);
            IDictionary<TKey, TValue> map = new Dictionary<TKey, TValue>();
            foreach (TValue? value in originalValues)
            {
                if (value != null)
                {
                    TKey key = GetKey(value);
                    if (key != null)
                    {
                        map.Add(key, value);
                    }
                }
            }
            var newValues = new List<TValue?>(keys.Count);
            foreach (TKey key in keys)
            {
                TValue? value;
                map.TryGetValue(key, out value);
                newValues.Add(value);
            }
            Func<IReadOnlyList<Result<TValue>>> asyncBody =
                () =>
                {
                    var query =
                        from value in newValues
                        select Result<TValue>.Resolve(value);
                    return query.ToList();
                };
            return Task.Run(asyncBody);
        }

        protected abstract TKey GetKey(TValue value);

        protected abstract IList<TValue> BatchLoad(IReadOnlyCollection<TKey> keys);
    }
}
