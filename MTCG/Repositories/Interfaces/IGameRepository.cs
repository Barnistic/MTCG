using MTCG.Models;
using MTCG.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories.Interfaces
{
    public interface IGameRepository
    {
        void CreateStats(User user);
        void UpdateStats(User user);
        StatDTO GetStat(string userId);
        List<StatDTO> GetScoreBoard();
    }
}
