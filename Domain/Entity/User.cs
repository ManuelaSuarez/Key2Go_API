namespace Domain.Entity
{
    public class User : BaseEntity
    {
        public string Dni { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        public int RoleId { get; set; } = 1;
        public Role Role { get; set; }
    }
}
