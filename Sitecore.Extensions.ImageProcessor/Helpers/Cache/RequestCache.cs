using System;
using System.Collections;

namespace Sitecore.Extensions.ImageProcessor.Helpers.Cache
{
    class RequestCache : ICache
    {
        private IDictionary _cache
        {
            get
            {
                if (null != System.Web.HttpContext.Current)
                    return System.Web.HttpContext.Current.Items;
                else
                    return new Hashtable();
            }
        }

        public bool Clear(string key)
        {
            _cache.Remove(key);
            return true;
        }

        public T GetOrAdd<T>(string key, Func<T> valFunc)
        {
            object obj;
            if (_cache.Contains(key))
                obj = _cache[key];
            else
            {
                obj = valFunc();
                _cache.Add(key, obj);
            }
            return (T)obj;
        }

        public bool RemoveAll()
        {
            _cache.Clear();
            return true;
        }


        public T GetOrAdd<T>(string key, Func<T> valFunc, int expire)
        {
            return GetOrAdd<T>(key, valFunc);
        }

        public T Set<T>(string key, Func<T> valFunc)
        {
            object obj;
            if (_cache.Contains(key))
            {
                _cache.Remove(key);
            }
            obj = valFunc();
            _cache.Add(key, obj);

            return (T)obj;
        }

        public T Get<T>(string key)
        {
            object obj;
            if (_cache.Contains(key))
            {
                obj = _cache[key];
            }
            else
            { obj = null; }
            return (T)obj;
        }

        public T GetOrAdd<T>(string key, Func<T> valFunc, bool force)
        {
            if (force)
                return valFunc();
            return GetOrAdd(key, valFunc);
        }
    }
}
