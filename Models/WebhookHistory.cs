using System;
using System.Collections.Generic;

namespace DataCloudDemo.Models
{
    public partial class WebhookHistory
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Eventtype { get; set; }
        public DateTime? Createdate { get; set; }
        public DateTime? Dataactioninitiatedon { get; set; }
        public DateTime? Platformeventinvokedon { get; set; }
        public DateTime? Functioninvokedon { get; set; }
        public DateTime? Contactupdatedon { get; set; }
        public string EventId { get; set; }
        public string Contactid { get; set; }
    }
}
