using System;

namespace Guldkortet
{
    // Kort klassen och dess properties
    public class Card
    {
        // Properties för kortdata
        public string CardId { get; set; }
        public bool IsGoldCard { get; set; }
        public bool IsUsed { get; set; }

        public Card(string cardId, bool isGoldCard, bool isUsed)
        {
            CardId = cardId;
            IsGoldCard = isGoldCard;
            IsUsed = isUsed;
        }
    }
}
