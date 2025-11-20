using Application.Abstraction;
using Application.Abstraction.ExternalService;
using Application.Service.Helpers.Normalitazion;
using Application.Service.Helpers.Validations;
using Contract.External.Auth.Request;
using Contract.External.Auth.Response;
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
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var response = new LoginResponse { Message = "" };


            var authenticatedUser = await _userRepository.AuthenticatorAsync(request.Email, request.Password);
            if (authenticatedUser == null)
            {
                response.Message = "Credenciales inválidas.";
                return response;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, authenticatedUser.Id.ToString()),
                new Claim(ClaimTypes.Role, authenticatedUser.Role.Name.ToString()),
                new Claim(ClaimTypes.Email, authenticatedUser.Email)
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

            return new LoginResponse
            {
                Message = "Inicio de sesión exitoso.",
                Token = tokenString
            };
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetByEmail(request.Email);
            if (existingUser != null)
            {
                throw new Exception($"The Email {request.Email} is already in use");
            }

            var existingDniUser = await _userRepository.GetByDni(request.Dni);

            if (existingDniUser != null)
            {
                throw new Exception($"User with DNI {request.Dni} already exists");
            }

            if (!(UserValidations.DniValidation(request.Dni)))
            {
                throw new Exception("invalid DNI format");
            }
            if (!(UserValidations.EmailValidation(request.Email)))
            {
                throw new Exception("invalid Email format");
            }
            if (!(UserValidations.PasswordValidation(request.Password)))
            {
                throw new Exception("Your password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.");
            }
            if (!(UserValidations.PhoneNumberValidation(request.PhoneNumber)))
            {
                throw new Exception("invalid phonenumber format");
            }

            var user = new User
            {
                Dni = TextNormalization.Normalization(request.Dni),
                Name = TextNormalization.Normalization(request.Name),
                Surname = TextNormalization.Normalization(request.Surname),
                Email = TextNormalization.Normalization(request.Email),
                Password = request.Password,
                PhoneNumber = TextNormalization.Normalization(request.PhoneNumber),
                RoleId = (int)RoleType.User
            };

            user = await _userRepository.CreateAsync(user);

            return new RegisterResponse
            {
                Id = user.Id,
                Dni = user.Dni,
                CompleteName = $"{user.Surname}, {user.Name}",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId
            };
        }
    }
}
