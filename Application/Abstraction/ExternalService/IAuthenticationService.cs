using Contract.External.Auth.Request;
using Contract.External.Auth.Response;

namespace Application.Abstraction.ExternalService
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<RegisterResponse> Register(RegisterRequest request);
    }
}
