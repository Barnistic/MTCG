using MTCG.Models;
using MTCG.Services.Interfaces;
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

        public void AddRandomCard(int amount, User user)
        {
            for (int i = 0; i < amount; i++)
            {
                user.Stack.Add(Card.CreateRandomCard());
            }
        }

        public void RemoveCard(User user, int selection)
        {
            user.Stack.Remove(user.Stack[selection]);
        }

        public void MoveCard(User user, int i, int j)
        {
            Card temp = user.Stack[i];
            user.Stack[i] = user.Stack[j];
            user.Stack[j] = temp;
        }

        public void UpdateDeck(User user)
        {
            user.Deck = user.Stack.Take(5).ToList();
        }

        public void PrintCardStack(User user)
        {
            int i = 1;
            foreach (Card card in user.Stack)
            {
                Console.WriteLine(i + ": " + card);
                i++;
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
