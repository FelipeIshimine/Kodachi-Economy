using System;
using System.Collections.Generic;

namespace KodachiGames.Economy
{
    [Serializable]
    public class InventorySaveData
    {
        public List<string> OwnedProductIds = new();
    }
}
