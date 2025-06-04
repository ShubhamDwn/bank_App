using System.Text;
using System.Security.Cryptography;


namespace bank_demo.Services
{
    class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                foreach (var b in bytes)
                    builder.Append(b.ToString("x2")); // convert byte to hex  

                return builder.ToString();
            }
        }
    }
}
