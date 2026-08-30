using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guldkortet
{
    // Egen listklass baserad på en array
    public class MyRewardList
    {
        private Reward[] items;
        private int count;

        public int Count
        {
            get { return count; }
        }

        public MyRewardList()
        {
            // Startar med plats för 4 belöningar
            items = new Reward[4];
            count = 0;
        }

        // Lägger till ett element i listan
        public void Add(Reward reward)
        {
            // Om arrayen är full, dubblera storleken
            if (count == items.Length)
            {
                ExpandArray();
            }

            items[count] = reward;
            count++;
        }

        // Hjälpmetod för att förstora arrayen när den blir full
        private void ExpandArray()
        {
            Reward[] tempArray = new Reward[items.Length * 2];
            for (int i = 0; i < items.Length; i++)
            {
                tempArray[i] = items[i];
            }
            items = tempArray;
        }

        // Hämtar ett element via ett index (t.ex. mylist.Get(0))
        public Reward Get(int index)
        {
            if (index >= 0 && index < count)
            {
                return items[index];
            }
            return null;
        }

        // Returnerar en kopia av alla sparade belöningar (upp till count, inte hela den interna arrayens kapacitet)
        public Reward[] GetAll()
        {
            Reward[] result = new Reward[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = items[i];
            }
            return result;
        }
    }
}
