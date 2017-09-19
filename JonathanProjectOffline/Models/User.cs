using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonathanProjectOffline.Models
{
    public class User
    {
        public String UserName { get; set; }
        public String Password { get; set; }

        public User(string username, string password)
        {
            UserName = username;
            Password = password;
        }
    }
}
