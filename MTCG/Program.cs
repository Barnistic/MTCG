using MTCG.Models;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User.Register("TestName", "Testpass");

            Card newCard = Card.CreateRandomCard();
            Console.WriteLine(newCard);
        }
    }
}
