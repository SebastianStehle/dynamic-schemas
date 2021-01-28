using HotChocolate;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication1
{
    public sealed class CachingRequestExecutorResolver : IRequestExecutorResolver
    {
        private readonly IRequestExecutorResolver inner;
        private readonly IActionContextAccessor actionContextAccessor;
        private readonly IMemoryCache memoryCache;

        public event EventHandler<RequestExecutorEvictedEventArgs> RequestExecutorEvicted
        {
            add
            {
                inner.RequestExecutorEvicted += value;
            }
            remove
            {
                inner.RequestExecutorEvicted -= value;
            }
        }

        public CachingRequestExecutorResolver(IRequestExecutorResolver inner, 
            IActionContextAccessor actionContextAccessor, IMemoryCache memoryCache)
        {
            this.inner = inner;

            this.actionContextAccessor = actionContextAccessor;

            this.memoryCache = memoryCache;
        }

        public void EvictRequestExecutor(NameString schemaName = default)
        {
            inner.EvictRequestExecutor(schemaName);
        }

        public async ValueTask<IRequestExecutor> GetRequestExecutorAsync(NameString schemaName = default, CancellationToken cancellationToken = default)
        {
            var name = (string)actionContextAccessor.ActionContext.RouteData.Values["app"];

            return await memoryCache.GetOrCreateAsync(name, async x =>
            {
                x.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                x.RegisterPostEvictionCallback(OnCacheEvicted, name);

                var result =  await inner.GetRequestExecutorAsync(name, cancellationToken);

                return result;
            });
        }

        private void OnCacheEvicted(object key, object value, EvictionReason reason, object state)
        {
            inner.EvictRequestExecutor((string)state);
        }
    }
}
