using System;
using System.Collections.Generic;

namespace KodachiGames.Economy
{
    [Serializable]
    public class WalletSaveData
    {
        public List<CurrencyBalance> Balances = new();

        [Serializable]
        public class CurrencyBalance
        {
            public string CurrencyId;
            public int Amount;
        }
    }
}
