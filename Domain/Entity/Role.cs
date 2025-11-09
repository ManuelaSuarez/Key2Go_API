namespace Domain.Entity
{
    public class Role : BaseEntity
    {
        public RoleType Name { get; set; }
    }

    public enum RoleType
    {
        User = 1,
        Admin = 2,
        SuperAdmin = 3
    }
}
