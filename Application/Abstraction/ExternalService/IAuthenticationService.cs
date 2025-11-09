using Contract.Auth.Request;

namespace Application.Abstraction.ExternalService
{
    public interface IAuthenticationService
    {
        Task<string> Login(LoginRequest request);
    }
}
