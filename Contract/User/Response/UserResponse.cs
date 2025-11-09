namespace Contract.User.Response
{
    public class UserResponse
    {
        public int Id {  get; set; } 
        public string Dni { get; set; }
        public string CompleteName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int RoleId { get; set; }
    }
}
