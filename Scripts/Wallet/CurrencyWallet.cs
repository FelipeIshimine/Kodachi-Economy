using System;
using System.Collections.Generic;
using System.Linq;

namespace KodachiGames.Economy
{
    public class CurrencyWallet
    {
        private readonly Dictionary<string, int> _balances = new();

        public event Action<CurrencyDefinition, int> OnBalanceChanged;

        public int GetBalance(CurrencyDefinition currency) =>
            _balances.TryGetValue(currency.Id, out var balance) ? balance : 0;

        public bool TrySpend(CurrencyDefinition currency, int amount)
        {
            if (GetBalance(currency) < amount) return false;
            _balances[currency.Id] -= amount;
            OnBalanceChanged?.Invoke(currency, _balances[currency.Id]);
            return true;
        }

        public void Credit(CurrencyDefinition currency, int amount)
        {
            _balances[currency.Id] = GetBalance(currency) + amount;
            OnBalanceChanged?.Invoke(currency, _balances[currency.Id]);
        }

        internal WalletSaveData ToSaveData() => new()
        {
            Balances = _balances
                .Select(kvp => new WalletSaveData.CurrencyBalance { CurrencyId = kvp.Key, Amount = kvp.Value })
                .ToList()
        };

        internal void LoadFromSaveData(WalletSaveData data)
        {
            _balances.Clear();
            foreach (var entry in data.Balances)
                _balances[entry.CurrencyId] = entry.Amount;
        }
    }
}
