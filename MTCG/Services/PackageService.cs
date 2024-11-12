using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    internal class PackageService
    {
        const int Price = 5;
        public bool BuyPackage(User user)
        {
            if (user.Coins > Price)
            {
                Card[] newCards;
            }

            return false;
        }
    }
}
