using Newtonsoft.Json;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Extensions.ImageProcessor.Entities;
using Sitecore.Extensions.ImageProcessor.Helpers;
using Sitecore.Pipelines.Upload;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Sitecore.Extensions.ImageProcessor
{
    public class KrakenImageProcessor : Save
    {
        static Uri _krakenApiBase = new Uri(Utility.KrakenAPIBase);
        static string _actionUrl = Utility.KrakenAPIURL;

        static string KrakenKey = Utility.KrakenKey;
        static string KrakenSecret = Utility.KrakenSecret;

        public new void Process(UploadArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            bool enabled = string.Equals(Utility.KrakenProcessionEnabled.ToLower(), "1");
            if (enabled)
            {
                Log.Info("Starting Image Kraken Processing", this);
                foreach (string index in args.Files)
                {
                    HttpPostedFile file = args.Files[index];
                    var fileExtn = Path.GetExtension(file.FileName);
                    Log.Info(String.Format("File extension: {0}", fileExtn), this);
                    if (!string.IsNullOrEmpty(file.FileName) && IsValidImageRequest(fileExtn))
                    {
                        //Kraken the image                    
                        Log.Info(String.Format("Starting to Krak the image - {0}", file.FileName), this);
                        var krakedImg = GetKrakenedImage(file);
                        if (!string.IsNullOrEmpty(krakedImg))
                        {
                            Log.Info(String.Format("Uploading the Kraked image from location - {0}", krakedImg), this);
                            UploadImage(args, file, krakedImg);
                        }
                        else
                        {
                            Log.Info("Could not Krak the image", this);
                            base.Process(args);
                        }
                    }
                    else
                    {
                        base.Process(args);
                    }
                }
            }
            else
            {
                base.Process(args);
            }
        }

        private void UploadImage(UploadArgs args, HttpPostedFile file, string krakedImgePath)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(file, "file");

            var newFile = DownloadAndCreateFile(krakedImgePath, file);
            if (newFile == null)
            {
                base.Process(args);
            }
            else
            {
                bool flag = UploadProcessor.IsUnpack(args, newFile);
                var altText = args.GetFileParameter(file.FileName, "alt");

                MediaUploader mediaUploader = new MediaUploader
                {
                    File = newFile,
                    Unpack = flag,
                    Folder = args.Folder,
                    Versioned = args.Versioned,
                    Language = args.Language,
                    AlternateText = !(string.IsNullOrEmpty(altText)) ? altText : file.FileName,
                    Overwrite = args.Overwrite,
                    FileBased = args.Destination == UploadDestination.File
                };
                System.Collections.Generic.List<MediaUploadResult> list;
                using (new SecurityDisabler())
                {
                    list = mediaUploader.Upload();
                }
                Log.Audit(this, "Upload: {0}", new string[]
                            {
                                file.FileName
                            });
                foreach (MediaUploadResult current in list)
                {
                    this.ProcessItem(args, current.Item, current.Path);
                }
            }
        }

        //Sitecore default method
        private void ProcessItem(UploadArgs args, MediaItem mediaItem, string path)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(mediaItem, "mediaItem");
            Assert.ArgumentNotNull(path, "path");
            if (args.Destination == UploadDestination.Database)
            {
                Log.Info("Media Item has been uploaded to database: " + path, this);
            }
            else
            {
                Log.Info("Media Item has been uploaded to file system: " + path, this);
            }
            args.UploadedItems.Add(mediaItem.InnerItem);
        }

        /// <summary>
        /// Method to create HttpPostedFile with all the required parametes and properties
        /// </summary>
        /// <param name="fileToDownload"></param>
        /// <param name="origFile"></param>
        /// <returns></returns>
        private static HttpPostedFile DownloadAndCreateFile(string fileToDownload, HttpPostedFile origFile)
        {
            HttpPostedFile newFile = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(fileToDownload);
                client.Timeout = TimeSpan.FromMinutes(5);

                var request = new HttpRequestMessage(HttpMethod.Get, fileToDownload);
                var sendTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var response = sendTask.Result.EnsureSuccessStatusCode();
                var httpbytes = response.Content.ReadAsByteArrayAsync().Result;

                newFile = Utility.ConstructHttpPostedFile(httpbytes, origFile.FileName, origFile.ContentType);
            }
            return newFile;
        }

        public bool IsValidImageRequest(string mimeType)
        {
            var validImgExtns = Utility.ImageExtensions;
            IEnumerable<string> imageTypes;
            imageTypes = validImgExtns.Split(new[] { ",", "|", ";" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in imageTypes)
            {
                if (string.Equals(item.ToLower(), mimeType.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetKrakenedImage(HttpPostedFile imageFile)
        {
            var krakedImgUrl = string.Empty;
            try
            {
                var krackenapi = new KrackenApiReq
                {
                    Auth = new Authentication { ApiKey = KrakenKey, ApiSecret = KrakenSecret },
                    Dev = false,
                    Wait = true,
                    Lossy = true
                };
                var str = JsonConvert.SerializeObject(krackenapi);

                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));

                content.Add(new StringContent(str, Encoding.UTF8, "application/json"));
                content.Add(new StreamContent(imageFile.InputStream), imageFile.FileName, imageFile.FileName);

                var hClient = new HttpClient { BaseAddress = _krakenApiBase };

                using (var postData = hClient.PostAsync(_krakenApiBase + _actionUrl, content).Result)
                {
                    if (postData.IsSuccessStatusCode)
                    {
                        var response = postData.Content.ReadAsAsync<KrackenApiResp>();
                        krakedImgUrl = response.Result.Url;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Error kraking image - {0}{1}{2}", imageFile.FileName, Environment.NewLine, ex), this);
            }
            return krakedImgUrl;
        }
    }
}

