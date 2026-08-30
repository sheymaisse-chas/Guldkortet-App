using System;

namespace Guldkortet
{
    // En egen exception-klass som kastas när inkommande kod har ett ogiltigt format.
    public class InvalidCodeFormatException : GuldkortetException
    {
        public InvalidCodeFormatException()
            : base("Koden uppfyller inte det förväntade formatet (t.ex. AnvändarNr-KortNr).") { }
        public InvalidCodeFormatException(string message)
            : base(message) { }
        public InvalidCodeFormatException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
