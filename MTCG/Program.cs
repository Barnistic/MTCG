using MTCG.Models;
using MTCG.Services;
using MTCG.Interfaces;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {

            GameService Game = new();
            Server server = new(10001);
            //server.Start();
            Game.StartGame();

        }
    }
}
