using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonathanProjectOffline.Models
{
    public static class UserManager
    {
        public static List<User> Users = new List<User>();

        public static void PopulateUserlist()
        {
            Users.Add(new User("jonathan", "knutsson"));
            Users.Add(new Models.User("marita", "mansson"));
        }

        public static User CreateUser(string username, string password)
        {
            if (MatchByUserName(username))
            {
                throw new Exception("Username is taken");
            }else
            {
                User user = new User(username, password);
                return user;
            }
        }


        public static bool MatchByUserName(string username)
        {
            foreach (var item in Users)
            {
                if (item.UserName.Equals(username))
                    return true;
            }
            return false;
        }

        public static bool Login(string username, string password)
        {
            if (MatchByUserName(username))
            {
                foreach (var item in Users)
                {
                    if (item.UserName.Equals(username))
                    {
                        if (item.Password.Equals(password))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                throw new System.ArgumentException(String.Format("The username: {0}, does not exist in the database", username));
                //throw new Exception(String.Format("The username: {0}, does not exist in the database", username));
            }
            
        }

    }
}
