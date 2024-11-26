using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interfaces
{
    public interface ICardService
    {
        void AddCard(User user, Card card);
        void AddCard(User user, List<Card> cards);
        void AddRandomCard(int amount, User user);
        void RemoveCard(User user, int selection);
        void MoveCard(User user, int i, int j);
        void UpdateDeck(User user);
        void PrintCardStack(User user);
        void PrintCardDeck(User user);
    }
}
