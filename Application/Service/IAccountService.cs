using Contract.Account.Request;
using Contract.User.Response;

namespace Application.Service
{
    public interface IAccountService
    {
        Task<UserResponse> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
