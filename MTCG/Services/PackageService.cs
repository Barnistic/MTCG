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
            if (user.Coins > PACKAGE_PRICE)
            {
                user.Coins -= PACKAGE_PRICE;
                List<Card> newCards = new();
                for (int i = 0; i > PACKAGE_SIZE; i++)
                {
                    Card newCard = Card.CreateRandomCard();
                    newCards.Add(newCard);
                }
                return newCards;
            } else
            {
                return null;
            }

            return null;
        }
    }
}
