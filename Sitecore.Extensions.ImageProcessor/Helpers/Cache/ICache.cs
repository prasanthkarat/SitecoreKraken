using System;

namespace Sitecore.Extensions.ImageProcessor.Helpers.Cache
{
    public interface ICache
    {
        T GetOrAdd<T>(string key, Func<T> valFunc);
        T GetOrAdd<T>(string key, Func<T> valFunc, bool force);
        T GetOrAdd<T>(string key, Func<T> valFunc, int expire);
        bool Clear(string key);
        bool RemoveAll();

        T Set<T>(string key, Func<T> valFunc);
        T Get<T>(string key);
    }
}
