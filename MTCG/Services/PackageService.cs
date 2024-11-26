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
        public static bool BuyPackage(User user)
        {
            CardService cardService = new();
            if (user.Coins < PACKAGE_PRICE)
            {
                Console.WriteLine("Not enough coins!");
                return false;
            }

            user.Coins -= PACKAGE_PRICE;
            Console.WriteLine("Bought a package for 5 Coins!");
            List<Card> newCards = new();

            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
               newCards.Add(Card.CreateRandomCard());
            }

            cardService.AddCard(user, newCards);

            return true;
        }
    }
}
