namespace DataCloudDemo.ViewModels
{
    public class WebhookHerokuClickEventViewModel
    {
        public DateTime creationDateTime { get; set; }
        public int count { get; set; }
        public List<CDCSchema> schemas { get; set; }
        public List<CDCEvent> events { get; set; }
    }

    public class CDCSchema
    {
        public string schemaId { get; set; }
        public dynamic schema { get; set; }
    }

    public class CDCEvent
    {
        public string EventType { get; set; }
        public string SourceObjectDeveloperName { get; set; }
        public string EventPrompt { get; set;}
        public string ActionDeveloperName { get; set; }
        public string EventCreationDateTime { get; set; }
        public string EventPublishDateTime { get; set; }
        public string PayloadCurrentValue { get; set; }
        public string PayloadPrevValue { get; set;}
    }
}
