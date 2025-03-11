using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace RadioBD
{
    class MD5_hash
    {
        public static string HashPassword(string password)
        {
            MD5 md5 = MD5.Create();

            byte[] bytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (var h in hash)
            {
                sb.Append(h.ToString("X2"));
            }
            return Convert.ToString(sb);
        }
    }
}
