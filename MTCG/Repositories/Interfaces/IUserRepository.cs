using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetUsers();
        void AddUser(User user);
    }
}
