using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace BlazorAppEvergreenOIDC.Models.ScimModels
{
    public class ResourceUserDefault
    {
        public string id { get; set; } = string.Empty;
        [Required(ErrorMessage = "Username cannot be empty")]
        public string userName { get; set; } = string.Empty;
        public List<string>? schemas { get; set; }
        public bool active { get; set; }
        public List<SCIMAttribute>? emails { get; set; }
        public List<SCIMAttribute>? phoneNumbers { get; set; }
        public Name? name { get; set; }
        public string agreeToTerms { get; set; } = string.Empty;
        [JsonPropertyName("urn:se:curity:scim:2.0:Devices")]
        public List<ResourceDevice>? urnDevices { get; set; }
        public Meta? meta { get; set; }
    }
    public class Name
    {
        public string givenName { get; set; } = string.Empty;
        public string familyName { get; set; } = string.Empty;


    }
    public class SCIMAttribute
    {
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
        [JsonPropertyName("primary")]
        public bool Primary { get; set; }
    }
}
