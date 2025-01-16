using MTCG.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Damage { get; set; }
        public string Type { get; set; }

        public Card(string cardName, int cardDamage, string cardType)
        {
            Name = cardName;
            Damage = cardDamage;
            Type = cardType;
            Id = Guid.NewGuid().ToString();
        }

        public Card(CardDTO cardDto)
        {
            this.Name = cardDto.Name;
            if (cardDto.Name.StartsWith("Fire"))
            {
                this.Type = "Fire";
            }
            else if (cardDto.Name.StartsWith("Water"))
            {
                this.Type = "Water";
            }
            else
            {
                this.Type = "Regular";
            }
            this.Damage = cardDto.Damage;
            this.Id = cardDto.Id;
        }
    }
}
