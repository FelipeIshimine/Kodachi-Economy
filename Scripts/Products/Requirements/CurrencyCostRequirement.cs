using System;
using UnityEngine;

namespace KodachiGames.Economy
{
    [SelectorName("Requirements/Currency Cost")]
    [Serializable]
    public class CurrencyCostRequirement : IAcquisitionRequirement
    {
        [SerializeField] private CurrencyDefinition currency;
        [SerializeField] private int amount;

        public bool IsMet(IWalletRegistry wallets, WalletKey walletKey) =>
            wallets.Get(walletKey).GetBalance(currency) >= amount;

        public void Fulfill(IWalletRegistry wallets, WalletKey walletKey) =>
            wallets.Get(walletKey).TrySpend(currency, amount);
    }
}
