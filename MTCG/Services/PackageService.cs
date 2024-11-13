using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class PackageService
    {
        private const int PACKAGE_PRICE = 5;
        private const int PACKAGE_SIZE = 5;
        public static List<Card> BuyPackage(User user)
        {
            if (user.Coins < PACKAGE_PRICE)
            {
                throw new InvalidOperationException("You do not have enough coins to buy this package.");
            }

            user.Coins -= PACKAGE_PRICE;
            Console.WriteLine("Bought a package for 5 Coins!");
            List<Card> newCards = new();

            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                newCards.Add(Card.CreateRandomCard());
            }

            return newCards;
        }
    }
}
