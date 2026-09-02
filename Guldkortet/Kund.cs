using System;

namespace Guldkortet
{
    // Kund klassen och dess properties
    public class Kund
    {
        // Properties för kunddata
        public string AnvändarNr { get; set;}
        public string Namn { get; set; }
        public string Kommun { get; set; }

        public Kund(string användarNr, string namn, string kommun)
        {
            AnvändarNr = användarNr;
            Namn = namn;
            Kommun = kommun;
        }
    }
}
