using System;
using System.IO;
using System.Reflection;
using System.Web;

namespace Sitecore.Extensions.ImageProcessor.Helpers
{
    public static class Utility
    {
        public static string KrakenAPIBase { get { return SitecoreUtilities.KrakenSettings(KrakenConstants.KrakenAPIBase); } }
        public static string KrakenProcessionEnabled { get { return SitecoreUtilities.KrakenSettings(KrakenConstants.KrakenProcessionEnabled); } }
        public static string KrakenAPIURL { get { return SitecoreUtilities.KrakenSettings(KrakenConstants.KrakenAPIURL); } }
        public static string KrakenKey { get { return SitecoreUtilities.KrakenSettings(KrakenConstants.KrakenKey); } }
        public static string KrakenSecret { get { return SitecoreUtilities.KrakenSettings(KrakenConstants.KrakenSecret); } }
        public static string ImageExtensions { get { return SitecoreUtilities.KrakenSettings(KrakenConstants.ImageExtensions); } }


        /// <summary>
        /// From Stackoverflow: http://stackoverflow.com/questions/5514715/how-to-instantiate-a-httppostedfile
        /// This is the best method available to create an HttpPostedFile object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static HttpPostedFile ConstructHttpPostedFile(byte[] data, string filename, string contentType)
        {
            // Get the System.Web assembly reference
            Assembly systemWebAssembly = typeof(HttpPostedFileBase).Assembly;
            // Get the types of the two internal types we need
            Type typeHttpRawUploadedContent = systemWebAssembly.GetType("System.Web.HttpRawUploadedContent");
            Type typeHttpInputStream = systemWebAssembly.GetType("System.Web.HttpInputStream");

            // Prepare the signatures of the constructors we want.
            Type[] uploadedParams = { typeof(int), typeof(int) };
            Type[] streamParams = { typeHttpRawUploadedContent, typeof(int), typeof(int) };
            Type[] parameters = { typeof(string), typeof(string), typeHttpInputStream };

            // Create an HttpRawUploadedContent instance
            object uploadedContent = typeHttpRawUploadedContent
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, uploadedParams, null)
              .Invoke(new object[] { data.Length, data.Length });

            // Call the AddBytes method
            typeHttpRawUploadedContent
              .GetMethod("AddBytes", BindingFlags.NonPublic | BindingFlags.Instance)
              .Invoke(uploadedContent, new object[] { data, 0, data.Length });

            // This is necessary if you will be using the returned content (ie to Save)
            typeHttpRawUploadedContent
              .GetMethod("DoneAddingBytes", BindingFlags.NonPublic | BindingFlags.Instance)
              .Invoke(uploadedContent, null);

            // Create an HttpInputStream instance
            object stream = (Stream)typeHttpInputStream
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, streamParams, null)
              .Invoke(new object[] { uploadedContent, 0, data.Length });

            // Create an HttpPostedFile instance
            HttpPostedFile postedFile = (HttpPostedFile)typeof(HttpPostedFile)
              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, parameters, null)
              .Invoke(new object[] { filename, contentType, stream });

            return postedFile;
        }
    }
}
