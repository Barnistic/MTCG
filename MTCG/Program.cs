using MTCG.Models;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User testUser = new User("testName", "testPassword");
            Console.WriteLine(testUser);
            MonsterCard testMonsterCard = new MonsterCard("testMonsterCardName", "testMonsterCardType");
            Console.WriteLine(testMonsterCard);
        }
    }
}
