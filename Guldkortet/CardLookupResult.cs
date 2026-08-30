using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guldkortet
{
    // Objekt som visar tydligt vad som finns i resultatet av en kortsökning i databasen
    public class CardLookupResult
    {
        public bool Found { get; set; }
        public bool IsGoldCard { get; set; }
        public bool IsUsed { get; set; }
        public string CardName { get; set; }
    }
}
