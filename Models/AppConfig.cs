using System;
using System.Collections.Generic;

namespace DataCloudDemo.Models
{
    public partial class AppConfig
    {
        public Guid Id { get; set; }
        public string ContactId { get; set; }
        public string UserLocale { get; set; }
        public string UserTimezone { get; set; }
    }
}
