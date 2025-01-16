using MTCG.Models;
using MTCG.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG.Infrastructure
{
    public partial class RequestHandler
    {
        public void HandleStatsGet(User requester, NetworkStream stream)
        {
            Console.WriteLine("\nHandleStatsGet");
            StatDTO statDto = _gameRepository.GetStat(requester.Id);
            if (statDto != null)
            {
                string statsJson = JsonSerializer.Serialize(statDto);
                SendResponse(stream, "200 OK", statsJson);
            }
            else
            {
                SendResponse(stream, "404 Not Found", "Stats not found for the user");
            }
        }

        public void HandleScoreboardGet(NetworkStream stream)
        {
            Console.WriteLine("\nHandleScoreboardGet");
            var scoreboard = _gameRepository.GetScoreBoard();
            string scoreboardJson = JsonSerializer.Serialize(scoreboard);
            SendResponse(stream, "200 OK", scoreboardJson);
        }


        public void HandleBattlePost(User requester, NetworkStream stream)
        {
            Console.WriteLine("\nHandleBattlePost");
            lock (_battleLobby)
            {
                if (_battleLobby.Count > 0)
                {
                    var opponent = _battleLobby.Dequeue();
                    _battleService.Battle(requester, opponent);
                    var battleLog = _battleService.GetBattleLog();
                    var response = new
                    {
                        Message = "The battle has been carried out successfully",
                        BattleLog = battleLog
                    };
                    string responseJson = JsonSerializer.Serialize(response);
                    _gameRepository.UpdateStats(requester);
                    _gameRepository.UpdateStats(opponent);
                    SendResponse(stream, "200 OK", responseJson);
                }
                else
                {
                    _battleLobby.Enqueue(requester);
                    SendResponse(stream, "200 OK", "Waiting for an opponent...");
                }
            }
        }
    }
}
