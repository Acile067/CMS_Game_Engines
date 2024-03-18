using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Game_Engines.Common
{
    public enum UserRole
    {
        Visitor,
        Admin
    }
    [Serializable]
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
