namespace BlazorAppEvergreenOIDC.Models.ScimModels;

public class ResourceUser : ResourceUserDefault
{
    public string role { get; set; } = string.Empty;
    public string department { get; set; } = string.Empty;
    public string manager { get; set; } = string.Empty;
    public bool provisioned { get; set; }

    public string GetFirstPhone()
    {
        return phoneNumbers?.FirstOrDefault()?.Value ?? string.Empty;
    }
    public string GetFirstEmail()
    {
        return emails?.FirstOrDefault()?.Value ?? string.Empty;
    }


    public string GetGivenName()
    {
        return name?.givenName;
    }

    public string GetFamilyName()
    {
        return name?.familyName;
    }
}
