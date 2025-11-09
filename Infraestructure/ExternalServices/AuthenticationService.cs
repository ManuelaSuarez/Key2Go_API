using Application.Abstraction;
using Application.Abstraction.ExternalService;
using Azure.Core;
using Contract.Auth.Request;
using Contract.Auth.Response;
using Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infraestructure.ExternalServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;

        }
        public async Task<string> Login(LoginRequest request)
        {
            var response = new LoginResponse { Message="" };


            var authenticatedUser = await _userRepository.AuthenticatorAsync(request.Email, request.Password);
            if (authenticatedUser == null)
            {
                response.Message = "Credenciales inválidas.";
                return string.Empty;
            }

            var claims = new[]
            {
                new Claim("sub", authenticatedUser.Id.ToString()),
                new Claim(ClaimTypes.Role, authenticatedUser.Role.Name.ToString()),
                new Claim("email", authenticatedUser.Email)
            };

            var expireHours = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);
            var expires = DateTime.UtcNow.AddHours(expireHours);

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            response.Message = "Inicio de sesión exitoso.";
            return tokenString;
        }
    }
}
