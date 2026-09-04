using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guldkortet
{
    // Egen listklass baserad på en array
    public class MyList<T>
    {
        private T[] items;
        private int count;

        public int Count
        {
            get { return count; }
        }

        public MyList()
        {
            // Startar med plats för 4 belöningar
            items = new T[4];
            count = 0;
        }

        // Lägger till ett element i listan
        public void Add(T item)
        {
            // Om arrayen är full, dubblera storleken
            if (count == items.Length)
            {
                ExpandArray();
            }

            items[count] = item;
            count++;
        }

        // Hjälpmetod för att förstora arrayen när den blir full
        private void ExpandArray()
        {
            T[] tempArray = new T[items.Length * 2];
            for (int i = 0; i < items.Length; i++)
            {
                tempArray[i] = items[i];
            }
            items = tempArray;
        }

        // Hämtar ett element via ett index (t.ex. mylist.Get(0))
        public T Get(int index)
        {
            if (index >= 0 && index < count)
            {
                return items[index];
            }
            return default(T);
        }

        // Returnerar en kopia av alla sparade belöningar (upp till count, inte hela den interna arrayens kapacitet)
        public T[] GetAll()
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = items[i];
            }
            return result;
        }
    }
}
