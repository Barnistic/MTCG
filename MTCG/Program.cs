using MTCG.Models;
using MTCG.Services;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Services
            UserService userService = new();
            CardService cardService = new();

            User newUser = userService.Register("TestName", "Testpass");

            try
            {
                cardService.AddCard(newUser, PackageService.BuyPackage(newUser));
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
            

            cardService.PrintCardStack(newUser);
        }
    }
}
