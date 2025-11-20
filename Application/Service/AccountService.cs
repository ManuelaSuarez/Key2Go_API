using Application.Abstraction;
using Application.Service.Helpers.Normalitazion;
using Application.Service.Helpers.Validations;
using Contract.Account.Request;
using Contract.User.Response;
using Domain.Entity;

namespace Application.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse?> GetProfileAsync(int userId)
        {
            var response = await _userRepository.GetByIdAsync(userId) is User user ?
                    new UserResponse()
                    {
                        Id = user.Id,
                        Dni = user.Dni,
                        CompleteName = $"{user.Surname}, {user.Name}",
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        RoleId = user.RoleId,
                        IsActive = user.IsActive
                    } : null;

            return response;
        }

        public async Task<bool> UpdateProfileAsync(int id, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            var existingUser = await _userRepository.GetByEmail(request.Email);
            if (existingUser != null && existingUser.Id != id)
            {
                throw new Exception($"The Email {request.Email} is already in use");
            }

            var existingDniUser = await _userRepository.GetByDni(request.Dni);

            if (existingDniUser != null && existingDniUser.Id != id)
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
            if (!(UserValidations.PhoneNumberValidation(request.PhoneNumber)))
            {
                throw new Exception("invalid phonenumber format");
            }

            user.Dni = TextNormalization.Normalization(request.Dni);
            user.Name = TextNormalization.Normalization(request.Name);
            user.Surname = TextNormalization.Normalization(request.Surname);
            user.Email = TextNormalization.Normalization(request.Email);
            user.PhoneNumber = TextNormalization.Normalization(request.PhoneNumber);

            user = await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            if (user.Password != request.CurrentPassword)
                return false;

            if (!(UserValidations.PasswordValidation(request.NewPassword)))
            {
                throw new Exception("Your password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.");
            }

            user.Password = request.NewPassword;
            user = await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
