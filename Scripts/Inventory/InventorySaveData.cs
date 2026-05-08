using System;
using System.Collections.Generic;

namespace KodachiGames.Economy
{
    [Serializable]
    public class InventorySaveData
    {
        public List<Entry> Entries = new();

        [Serializable]
        public class Entry
        {
            public string ProductId;
            public int Count;
        }
    }
}
