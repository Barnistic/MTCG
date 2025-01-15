using MTCG.Models;
using MTCG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services.Interfaces
{
    public interface IBattleService
    {
        void Battle(User user1, User user2);
        double CalculateDamage(Card attacker, Card defender);
        string GetBattleLog();
    }
}
