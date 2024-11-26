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
        //private bool Authenticate(string username, string password);
    }
}
