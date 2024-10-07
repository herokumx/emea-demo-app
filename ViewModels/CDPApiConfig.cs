using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DataCloudDemo.ViewModels
{
    public class CDPApiConfig
    {
        [JsonIgnore]
        public string ccid { get; set; }
        public string configId { get; set; }

        [Required(ErrorMessage = "Configuration Name Is Required")]
        public string configName { get; set; }

        [Required(ErrorMessage = "Client Id Is Required")]
        public string clientId { get; set; }

        [Required(ErrorMessage = "Client Secret Is Required")]
        public string clientSecret { get; set; }
        [Required(ErrorMessage = "UserId Is Required")]
        public string userId { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        public string password { get; set; }

        public bool isDefault { get; set; } = false;

        public string sfEndpointUrl { get; set; }
    }
}
