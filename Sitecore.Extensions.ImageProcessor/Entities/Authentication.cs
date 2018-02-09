using Newtonsoft.Json;

namespace Sitecore.Extensions.ImageProcessor.Entities
{
    public class Authentication
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("api_secret")]
        public string ApiSecret { get; set; }
    }
}
