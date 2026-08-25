using System;

namespace Guldkortet
{
    // Kort klassen och dess properties
    public class Card
    {
        // Properties för kortdata
        public string CardId { get; set; }
        public string CardType { get; set; }
        public bool IsGoldCard { get; set; }
        public bool IsUsed { get; set; }

        public Card(string cardId, string cardType, bool isGoldCard, bool isUsed)
        {
            CardId = cardId;
            CardType = cardType;
            IsGoldCard = isGoldCard;
            IsUsed = isUsed;
        }
    }
}
