using MTCG.Interfaces;
using MTCG.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class CardService : ICardService
    {
        public void AddCard(User user, Card card)
        {
            user.Stack.Add(card);
        }

        public void AddCard(User user, List<Card> cards)
        {
            if (cards == null) return;
            user.Stack.AddRange(cards);
        }

        public void RemoveCard(User user, Card card)
        {
            user.Stack.Remove(card);
        }

        public void PrintCardStack(User user)
        {
            foreach (Card card in user.Stack)
            {
                Console.WriteLine(card);
            }
        }

        public void PrintCardDeck(User user)
        {
            foreach (Card card in user.Deck)
            {
                Console.WriteLine(card);
            }
        }
    }
}
