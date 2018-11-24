namespace ImageOrganiser.Application
{
    public class Settings
    {
        public string BucketName { get; set; }
        public string SourcePrefix { get; set; }
        public string DestinationPrefix { get; set; }
        public int? Age { get; set; }
        public bool? IsMale { get; set; }
        public bool? IsSmiling { get; set; }
    }
}
