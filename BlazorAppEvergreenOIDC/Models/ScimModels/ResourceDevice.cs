namespace BlazorAppEvergreenOIDC.Models.ScimModels
{
    using System;
    using System.Collections.Generic;

    public class ResourceDevice
    {
        public string DeviceType { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public Meta? Meta { get; set; }
        public List<string>? Schemas { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public Guid DeviceId { get; set; }


        public string GetCreated()
        {
            return Meta?.Created.ToString() ?? DateTime.Now.ToString();
        }

        public string GetLastModified()
        {
            return Meta?.LastModified.ToString() ?? DateTime.Now.ToString();
        }
    }
}
