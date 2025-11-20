using Application.Abstraction;
using Application.Service.Helpers.Normalitazion;
using Application.Service.Helpers.Validations;
using Contract.User.Request;
using Contract.User.Response;
using Domain.Entity;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; 

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserResponse>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var listUsers = users
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Dni = u.Dni,
                    CompleteName = $"{u.Surname}, {u.Name}",
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    RoleId = u.RoleId
                })
                .ToList();
            return listUsers;
        }
        public async Task<UserResponse?> GetById(int id)
        {
            var response = await _userRepository.GetByIdAsync(id) is User user ?
                    new UserResponse()
                    {
                        Id = user.Id,
                        Dni = user.Dni,
                        CompleteName = $"{user.Surname}, {user.Name}",
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        RoleId = user.RoleId
                    } : null;

            return response;
        }

        public async Task<UserResponse?> Create(UserRequest request)
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
            if (!(UserValidations.RoleValidation(request.RoleId)))
            {
                throw new Exception("Role can only be 1, 2 or 3");
            }

            var user = new User
            {
                Dni = TextNormalization.Normalization(request.Dni),
                Name = TextNormalization.Normalization(request.Name),
                Surname = TextNormalization.Normalization(request.Surname),
                Email = TextNormalization.Normalization(request.Email),
                Password = request.Password,
                PhoneNumber = TextNormalization.Normalization(request.PhoneNumber),
                RoleId = request.RoleId
            };

            user = await _userRepository.CreateAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                Dni = user.Dni,
                CompleteName = $"{user.Surname}, {user.Name}",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId
            };
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _userRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
            await _userRepository.DeleteAsync(entity);

            return true;
        }

        public async Task<UserResponse?> Update(int id, UserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            var existingUser = await _userRepository.GetByEmail(request.Email);
            if (existingUser != null && existingUser.Id != id)
                return null;

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
            if (!(UserValidations.PasswordValidation(request.Password)))
            {
                throw new Exception("Your password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.");
            }
            if (!(UserValidations.PhoneNumberValidation(request.PhoneNumber)))
            {
                throw new Exception("invalid phonenumber format");
            }
            if (!(UserValidations.RoleValidation(request.RoleId)))
            {
                throw new Exception("Role can only be 1, 2 or 3");
            }

            user.Dni = TextNormalization.Normalization(request.Dni);
            user.Name = TextNormalization.Normalization(request.Name);
            user.Surname = TextNormalization.Normalization(request.Surname);
            user.Email = TextNormalization.Normalization(request.Email);
            user.Password = request.Password;
            user.PhoneNumber = TextNormalization.Normalization(request.PhoneNumber);
            user.RoleId = request.RoleId;

            user = await _userRepository.UpdateAsync(user);

            return new UserResponse
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