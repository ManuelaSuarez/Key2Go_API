using System.Text.RegularExpressions;

namespace Application.Service.Helpers.Validations
{
    public static class UserValidations
    {
        public static bool DniValidation(string dni)
        {
            if (!(Regex.IsMatch(dni,@"^[1-9][0-9]{6,7}$")))
            {
                return false;
            };
            return true;
        }
        public static bool EmailValidation(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static bool PasswordValidation(string password)
        {
            if (!(Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$")))
            {
                return false;
            }
            return true;
        }
        public static bool PhoneNumberValidation(string phoneNumber)
        {
            if (!(Regex.IsMatch(phoneNumber, @"^[0-9]{10,12}$")))
            {
                return false;
            }
            return true;
        }
        public static bool RoleValidation(int role)
        {
            if(role < 1 || role > 3)
            {
                return false;
            }
            return true;
        }
    }
}
