using Application.Abstraction;
using Application.Service;
using Contract.User.Request;
using Contract.User.Response;
using Domain.Entity;

namespace Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; // Inyección para hacer uso del repo

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
                return null;

            var user = new User
            {
                Dni = request.Dni,
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId
            };

            user = await _userRepository.CreateAsync(user);  // Asigna el resultado

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

            // Verificar si el email ya existe en otro usuario
            var existingUser = await _userRepository.GetByEmail(request.Email);
            if (existingUser != null && existingUser.Id != id)
                return null;

            // Verificar si el DNI ya existe en otro usuario ojo al piojo

            // Actualizar los campos
            user.Dni = request.Dni;
            user.Name = request.Name;
            user.Surname = request.Surname;
            user.Email = request.Email;
            user.Password = request.Password;
            user.PhoneNumber = request.PhoneNumber;
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