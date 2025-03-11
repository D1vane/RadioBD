using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioBD
{
    /// 
    /// Проверка пользователя
    /// 
    public class checkRights
    {
        public string Login { get; set; }

        public bool Role { get; }

        public string Rights()
        {
            if (Role == true) return "Администратор";
            else return "Пользователь";

        }

        public checkRights (string login, bool role)
        {
            Login = login.Trim();
            Role = role;
        }
    }
}
