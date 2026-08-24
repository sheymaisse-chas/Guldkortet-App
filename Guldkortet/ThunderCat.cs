using System;

namespace Guldkortet
{
    public class ThunderCat : Reward
    {
        public int Speed { get; set; } = 120;
        public string Element { get; set; } = "Blixt";
        public string SpecialAttack { get; set; } = "Eld kula";

        // Konstruktor som sätter namn och beskrivning automatiskt om det finns
        public ThunderCat() : base("Dunderkatt", "En blixtsnabb katt med elektriska krafter!")
        {
        }

        public ThunderCat (int speed, string element, string specialAttack, string name, string description) : base (name, description)
        {
            Speed = speed;
            Element = element;
            SpecialAttack = specialAttack;
        }
        public override string GenerateMessage()
        {
            return $"Grattis! Du har vunnit {Name}: Element - {Element}, Hastighet - {Speed} km/h, Specialattack - {SpecialAttack}. {Description}";
        }
    }
}
