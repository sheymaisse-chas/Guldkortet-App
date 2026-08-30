using System;

namespace Guldkortet
{
    // Gemensam basklass för alla egna exceptions i Guldkortet-programmet
    public class GuldkortetException : Exception
    {
        public GuldkortetException()
            : base("Ett fel har inträffat i Guldkortet-programmet.") { }

        public GuldkortetException(string message)
            : base(message) { }

        public GuldkortetException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
