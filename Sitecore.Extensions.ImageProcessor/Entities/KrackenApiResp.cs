using Newtonsoft.Json;

namespace Sitecore.Extensions.ImageProcessor.Entities
{
    public class KrackenApiResp
    {
        [JsonProperty("file_name")]
        public string FileName { get; set; }
        [JsonProperty("original_size")]
        public int OriginalSize { get; set; }
        [JsonProperty("kraked_size")]
        public int KrakenedSize { get; set; }
        [JsonProperty("saved_bytes")]
        public int SizeReduction { get; set; }
        [JsonProperty("kraked_url")]
        public string Url { get; set; }
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
    }
}
