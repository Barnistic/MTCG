using MTCG.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class TradeEntry
    {
        public string Id { get; set; }
        public string CardToTrade { get; set; }
        public string Type { get; set; }
        public int MinimumDamage { get; set; }

        public TradeEntry() { }

        public TradeEntry(TradeEntryDTO dto)
        {
            this.Id = Guid.NewGuid().ToString();
            this.CardToTrade = dto.Card_id;
            this.Type = dto.ReqType;
            this.MinimumDamage = dto.ReqDamage;
        }
    }
}
