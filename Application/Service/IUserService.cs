using Contract.User.Request;
using Contract.User.Response;

namespace Application.Service
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetAll();
        Task<UserResponse?> GetById(int id);
        Task<UserResponse?> Create(UserRequest request);
        Task<bool> Delete(int id);
        Task<bool> ReactivateAsync(int id);
        Task<UserResponse?> Update(int id, UserRequest request);
    }
}