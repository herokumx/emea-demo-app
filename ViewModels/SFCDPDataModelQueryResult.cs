namespace DataCloudDemo.ViewModels
{
    public class SFCDPDataModelQueryResult
    {
        public List<Dictionary<string, object>> data { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public long rowCount { get; set; }
        public string queryId { get; set; }
        public bool done { get; set; }
    }
}
