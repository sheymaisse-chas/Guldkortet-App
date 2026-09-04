using System;
using System.Collections.Generic;

namespace Guldkortet
{
    internal class KortTracker
    {
        private List<string> användaKort = new List<string>();
        private object kortLock = new object();

        // Förser programmet med trådsäkert om kortet har använts
        public bool IsKortUsed(string kortNr)
        {
            lock (kortLock)
            {
                return användaKort.Contains(kortNr);
            }
        }

        // Lägger trådsäkert till ett kort
        public void MarkAsUsed(string kortNr)
        {
            lock (kortLock)
            {
                if (!användaKort.Contains(kortNr))
                {
                    användaKort.Add(kortNr);
                }
            }
        }
    }
}
