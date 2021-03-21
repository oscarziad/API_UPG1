using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace U1_API.Data
{
    public partial class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] UserHash { get; set; }
        public byte[] UserSalt { get; set; }


        public void CreatePasswordWithHash(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                UserSalt = hmac.Key;
                UserHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool ValidatePasswordHash(string password)
        {
            using (var hmac = new HMACSHA512(UserSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != UserHash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
