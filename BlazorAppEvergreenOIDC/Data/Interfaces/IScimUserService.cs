using BlazorAppEvergreenOIDC.Models.ScimModels;
using BlazorAppEvergreenOIDC.Models.ScimModels.Responses;

namespace BlazorAppEvergreenOIDC.Data.Interfaces
{
    public interface IScimUserService
    {
        Task<ResponseGetUsers> GetUsersAsync(int count, int startIndex);
        Task<ResponseGetUsers> GetUsersAsyncWithSearchQuery(int count, int startIndex, string searchQueryUserName);
        Task<ResponseGetUsers> GetUserByUsernameAsync(string username);
        Task<ResponseGetUserById> GetUserByIdAsync(string userId);
        Task<ResponseGetDevices> GetDevicesAsync(int count, int startIndex);
        Task DeleteDeviceAsync(string deviceId);
    }
}
