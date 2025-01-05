using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    internal class SpellCard : Card
    {
        public SpellCard(string name, int damage, string type) : base(name, damage, type)
        {
            
        }

        public override string ToString()
        {
            return $"Name: Spell {Name}, Damage: {Damage}, Type: {Type}";
        }
    }
}
