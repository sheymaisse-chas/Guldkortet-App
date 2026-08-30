using System;

namespace Guldkortet
{
    public class DatabaseConnectionException : GuldkortetException
    {
        public DatabaseConnectionException()
            : base("Kunde inte ansluta till databasen.") { }

        public DatabaseConnectionException(string message)
            : base(message) { }

        public DatabaseConnectionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
