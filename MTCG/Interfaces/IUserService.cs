using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interfaces
{
    public interface IUserService
    {
        User Register();
        User Login();
        void Win(User user);
        void Lose(User user);
        void AddUser(User user);
        List<User> GetUsers();
    }
}
