namespace BlazorAppEvergreenOIDC.Models.ScimModels.Responses;

public class ResponseGetUsers : ResponseDefault
{
    public IEnumerable<ResourceUser>? Resources { get; set; }
}
