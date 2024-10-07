using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DataCloudDemo.ViewModels
{
    public class AppSettingsViewModel
    {
        public Guid? Id { get; set; }

        [Required, Display(Name = "ContactId")]
        public string ContactId { get; set; }

        [Required, Display(Name = "User Locale")]
        public string Locale { get; set; }

        [Required, Display(Name = "User Timezone")]
        public string Timezone { get; set; }
    }
}
