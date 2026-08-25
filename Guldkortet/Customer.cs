using System;

namespace Guldkortet
{
    // Kund klassen och dess properties
    public class Customer
    {
        // Properties för kunddata
        public string CustomerId { get; set;}
        public string Name { get; set; }
        public string Municipality { get; set; }
        public string Email { get; set; }

        public Customer(string customerId, string name, string municipality, string email)
        {
            CustomerId = customerId;
            Name = name;
            Email = email;
            Municipality = municipality;
        }
    }
}
