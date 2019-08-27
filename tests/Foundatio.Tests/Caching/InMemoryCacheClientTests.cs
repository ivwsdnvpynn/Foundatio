﻿using System;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Utility;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Foundatio.Tests.Caching {
    public class InMemoryCacheClientTests : CacheClientTestsBase {
        public InMemoryCacheClientTests(ITestOutputHelper output) : base(output) {}

        protected override ICacheClient GetCacheClient() {
            return new InMemoryCacheClient(o => o.LoggerFactory(Log).CloneValues(true));
        }

        [Fact]
        public override Task CanGetAllAsync() {
            return base.CanGetAllAsync();
        }

        [Fact]
        public override Task CanGetAllWithOverlapAsync() {
            return base.CanGetAllWithOverlapAsync();
        }

        [Fact]
        public override Task CanSetAsync() {
            return base.CanSetAsync();
        }

        [Fact]
        public override Task CanSetAndGetValueAsync() {
            return base.CanSetAndGetValueAsync();
        }

        [Fact]
        public override Task CanAddAsync() {
            return base.CanAddAsync();
        }

        [Fact]
        public override Task CanAddConcurrentlyAsync() {
            return base.CanAddConcurrentlyAsync();
        }

        [Fact]
        public override Task CanTryGetAsync() {
            return base.CanTryGetAsync();
        }

        [Fact]
        public override Task CanUseScopedCachesAsync() {
            return base.CanUseScopedCachesAsync();
        }

        [Fact]
        public override Task CanSetAndGetObjectAsync() {
            return base.CanSetAndGetObjectAsync();
        }

        [Fact]
        public override Task CanRemoveByPrefixAsync() {
            return base.CanRemoveByPrefixAsync();
        }

        [Fact]
        public override Task CanSetExpirationAsync() {
            return base.CanSetExpirationAsync();
        }

        [Fact]
        public override Task CanSetMinMaxExpirationAsync() {
            return base.CanSetMinMaxExpirationAsync();
        }

        [Fact]
        public override Task CanIncrementAsync() {
            return base.CanIncrementAsync();
        }

        [Fact]
        public override Task CanIncrementAndExpireAsync() {
            return base.CanIncrementAndExpireAsync();
        }

        [Fact]
        public override Task CanReplaceIfEqual() {
            return base.CanReplaceIfEqual();
        }

        [Fact]
        public override Task CanRemoveIfEqual() {
            return base.CanRemoveIfEqual();
        }

        [Fact]
        public override Task CanGetAndSetDateTimeAsync() {
            return base.CanGetAndSetDateTimeAsync();
        }

        [Fact]
        public override Task CanRoundTripLargeNumbersAsync() {
            return base.CanRoundTripLargeNumbersAsync();
        }

        [Fact]
        public override Task CanRoundTripLargeNumbersWithExpirationAsync() {
            return base.CanRoundTripLargeNumbersWithExpirationAsync();
        }

        [Fact]
        public override Task CanManageSetsAsync() {
            return base.CanManageSetsAsync();
        }

        [Fact]
        public async Task CanSetMaxItems() {
            Log.MinimumLevel = LogLevel.Trace;

            // run in tight loop so that the code is warmed up and we can catch timing issues
            for (int x = 0; x < 5; x++) {
                var cache = new InMemoryCacheClient(o => o.MaxItems(10).CloneValues(true));

                using (cache) {
                    await cache.RemoveAllAsync();

                    for (int i = 0; i < cache.MaxItems; i++)
                        await cache.SetAsync("test" + i, i);

                    _logger.LogTrace(String.Join(",", cache.Keys));
                    Assert.Equal(10, cache.Count);
                    await cache.SetAsync("next", 1);
                    _logger.LogTrace(String.Join(",", cache.Keys));
                    Assert.Equal(10, cache.Count);
                    Assert.False((await cache.GetAsync<int>("test0")).HasValue);
                    Assert.Equal(1, cache.Misses);
                    await SystemClock.SleepAsync(50); // keep the last access ticks from being the same for all items
                    Assert.NotNull(await cache.GetAsync<int?>("test1"));
                    Assert.Equal(1, cache.Hits);
                    await cache.SetAsync("next2", 2);
                    _logger.LogTrace(String.Join(",", cache.Keys));
                    Assert.False((await cache.GetAsync<int>("test2")).HasValue);
                    Assert.Equal(2, cache.Misses);
                    Assert.True((await cache.GetAsync<int>("test1")).HasValue);
                    Assert.Equal(2, cache.Misses);
                }
            }
        }
    }
}