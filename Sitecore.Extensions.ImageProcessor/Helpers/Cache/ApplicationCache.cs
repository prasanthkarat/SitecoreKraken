using Sitecore.Configuration;
using System;
using System.Runtime.Caching;

namespace Sitecore.Extensions.ImageProcessor.Helpers.Cache
{
    public sealed class ApplicationCache : ICache, IDisposable
    {
        private ObjectCache _cache = new MemoryCache("krakenSettingCache");
        public bool Clear(string key)
        {
            return null != _cache.Remove(LangKey(key));
        }

        public void Dispose()
        {
            ((MemoryCache)_cache).Dispose();
        }

        public T GetOrAdd<T>(string key, Func<T> valFunc)
        {
            return GetOrAdd(key, valFunc, Settings.GetIntSetting(KrakenConstants.CacheTimeout, 30));
        }

        public bool RemoveAll()
        {
            _cache = new MemoryCache("krakenSettingCache");
            return true;
        }

        public T Set<T>(string key, Func<T> valFunc)
        {
            if (_cache.Contains(LangKey(key)))
            {
                _cache.Remove(LangKey(key));
            }
            CacheItemPolicy policy = new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(Settings.GetIntSetting(KrakenConstants.CacheTimeout, 30))
            };
            var val = valFunc();
            _cache.Add(LangKey(key), val, policy);

            return (T)val;
        }

        public T Get<T>(string key)
        {
            object obj;
            if (_cache.Contains(LangKey(key)))
                obj = (T)_cache.Get(LangKey(key));
            else
                obj = default(T);
            return (T)obj;
        }

        private string LangKey(string cacheKey)
        {
            if (null != Sitecore.Context.Language)
            {
                var name = Sitecore.Context.Language.Name;
                return name + cacheKey;
            }
            return cacheKey;
        }

        public T GetOrAdd<T>(string key, Func<T> valFunc, bool force)
        {
            return GetOrAdd(key, valFunc, Settings.GetIntSetting(KrakenConstants.CacheTimeout, 30), force);
        }

        T GetOrAdd<T>(string key, Func<T> valFunc, int expire, bool force)
        {
            if (Sitecore.Context.Item != null && Sitecore.Context.PageMode.IsPageEditor || force)
            {
                return valFunc();
            }
            if (_cache.Contains(LangKey(key)))
                return (T)_cache.Get(LangKey(key));
            else
            {
                CacheItemPolicy policy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(expire)
                };
                var val = valFunc();
                if (val == null)
                    return default(T);
                _cache.Add(LangKey(key), val, policy);
                return (T)val;
            }
        }

        public T GetOrAdd<T>(string key, Func<T> valFunc, int expire)
        {
            return GetOrAdd(key, valFunc, Settings.GetIntSetting(KrakenConstants.CacheTimeout, 30), false);
        }
    }
}
