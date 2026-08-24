using System;

namespace Guldkortet
{
    // Kund klassen och dess properties
    public class Customer
    {
        // Properties för kunddata
        public string CustomerId { get; set;}
        public string Name { get; set; }
        public string Email { get; set; }

        public Customer(string customerId, string name, string email)
        {
            CustomerId = customerId;
            Name = name;
            Email = email;
        }
    }
}
