namespace Sitecore.Extensions.ImageProcessor.Helpers.Cache
{
    public static class CacheHelper
    {
        static ICache _applicationCache;
        static ICache _requestCache;

        static CacheHelper()
        {
            _applicationCache = new ApplicationCache();
            _requestCache = new RequestCache();
        }

        public static ICache ApplicationCache { get { return _applicationCache; } }
        public static ICache RequestCache { get { return _requestCache; } }

        public static void ClearAll()
        {
            ApplicationCache.RemoveAll();
            RequestCache.RemoveAll();
        }
    }
}
