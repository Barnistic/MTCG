using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories.DTOs
{
    public class TradeEntryDTO
    {
        public string User_id { get; set; }
        public string Card_id { get; set; }
        public string ReqType { get; set; }
        public int ReqDamage { get; set; }
    }
}
