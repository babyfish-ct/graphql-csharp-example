using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace GraphQLCSharpExample.Loader.Common
{
    public abstract class AbstractValueLoader<TKey, TValue> :
        DataLoaderBase<TKey, TValue?>
        where TKey : notnull
        where TValue : class
    {
        private static readonly DataLoaderOptions<TKey> DEFAULT_OPTIONS =
            new DataLoaderOptions<TKey>
            {
                MaxBatchSize = 256
            };

        public AbstractValueLoader(DataLoaderOptions<TKey>? options = null)
            : base(options ?? DEFAULT_OPTIONS)
        { 
        }

        public Task<TValue?> LoadOptionalAsync(TKey key)
        {
            return LoadAsync(key, new CancellationToken());
        }

        public async Task<TValue> LoadRequiredAsync(TKey key)
        {
            return (await LoadAsync(key, new CancellationToken())) ?? throw new InvalidProgramException("Internal bug");
        }

        protected override sealed Task<IReadOnlyList<Result<TValue?>>> FetchAsync(
            IReadOnlyList<TKey> keys,
            CancellationToken cancellationToken)
        {
            IList<TValue> originalValues = BatchFetch(keys);
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
            Func<IReadOnlyList<Result<TValue?>>> asyncBody =
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

        protected abstract IList<TValue> BatchFetch(IReadOnlyCollection<TKey> keys);
    }
}
