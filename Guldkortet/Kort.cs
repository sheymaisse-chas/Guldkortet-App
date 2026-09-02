using System;

namespace Guldkortet
{
    // Kort klassen och dess properties
    public class Kort
    {
        // Properties för kortdata
        public string KortNr { get; set; }
        public string KortTyp { get; set; }

        public Kort(string kortNr, string kortTyp)
        {
            KortNr = kortNr;
            KortTyp = kortTyp;
        }
    }
}
