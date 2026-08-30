using System;

namespace Guldkortet
{
    // Underklassen Dunderkatt och dess properties
    public class ThunderCat : Reward
    {
        // Unika properties för Dunderkatt datan
        public int Speed { get; set; } = 120;
        public string Element { get; set; } = "Blixt";
        public string SpecialAttack { get; set; } = "Eld kula";

        // Konstruktor som sätter namn och beskrivning automatiskt om det finns
        public ThunderCat() : base("Dunderkatt", "En blixtsnabb katt med elektriska krafter!", "thundercat.png")
        {
        }

        public ThunderCat (int speed, string element, string specialAttack, string name, string description, string imagePath) : base (name, description, imagePath)
        {
            Speed = speed;
            Element = element;
            SpecialAttack = specialAttack;
        }
        public override string GenerateMessage()
        {
            return $"Grattis! Du har vunnit {Name}: Element - {Element}, Hastighet - {Speed} km/h, Specialattack - {SpecialAttack}. {ImagePath} {Description}";
        }
    }
}
