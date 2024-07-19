namespace BlazorAppEvergreenOIDC.Models.ScimModels.Responses
{
    public abstract class ResponseDefault
    {
        public int totalResults { get; set; }
        public int startIndex { get; set; }
        public int itemsPerPage { get; set; }
        public List<string>? schemas { get; set; }
    }
}
