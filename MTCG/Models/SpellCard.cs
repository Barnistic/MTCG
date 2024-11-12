using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    internal class SpellCard : Card
    {
        public SpellCard(string name, int damage, CardType type) : base(name, damage, type)
        {
            
        }
    }
}
