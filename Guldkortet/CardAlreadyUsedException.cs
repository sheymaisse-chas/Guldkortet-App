using System;

namespace Guldkortet
{
    // Kastas när ett Guldkort redan har lösts in tidigare.
    public class CardAlreadyUsedException : GuldkortetException
    {
        public CardAlreadyUsedException()
            : base("Kortet är ett giltigt Guldkort, men har redan lösts in.") { }

        public CardAlreadyUsedException(string message)
            : base(message) { }

        public CardAlreadyUsedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
