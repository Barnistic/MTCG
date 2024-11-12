using MTCG.Models;
using MTCG.Services;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User newUser = User.Register("TestName", "Testpass");

            PackageService.BuyPackage(newUser);

            newUser.PrintCardStack();
        }
    }
}
