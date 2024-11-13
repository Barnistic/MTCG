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
        void RemoveCard(User user, Card card);
        void PrintCardStack(User user);
        void PrintCardDeck(User user);
    }
}
