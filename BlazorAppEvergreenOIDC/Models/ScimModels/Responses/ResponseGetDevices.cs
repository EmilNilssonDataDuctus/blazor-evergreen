namespace BlazorAppEvergreenOIDC.Models.ScimModels.Responses;

public class ResponseGetDevices : ResponseDefault
{
    public IEnumerable<ResourceDevice>? Resources { get; set; }
}
