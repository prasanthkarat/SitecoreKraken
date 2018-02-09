using Newtonsoft.Json;

namespace Sitecore.Extensions.ImageProcessor.Entities
{
    public class KrackenApiReq
    {
        [JsonProperty("auth")]
        public Authentication Auth { get; set; }

        [JsonProperty("wait")]
        public bool Wait { get; set; }

        [JsonProperty("dev")]
        public bool Dev { get; set; }

        [JsonProperty("lossy")]
        public bool Lossy { get; set; }
    }
}
