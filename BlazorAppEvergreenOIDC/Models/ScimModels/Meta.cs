namespace BlazorAppEvergreenOIDC.Models.ScimModels
{
    public class Meta
    {
        public string ResourceType { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}
