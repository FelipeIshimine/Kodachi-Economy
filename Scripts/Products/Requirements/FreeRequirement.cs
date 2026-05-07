using System;

namespace KodachiGames.Economy
{
    [SelectorName("Requirements/Free")]
    [Serializable]
    public class FreeRequirement : IAcquisitionRequirement
    {
        public bool IsMet(IWalletRegistry wallets, WalletKey walletKey) => true;
        public void Fulfill(IWalletRegistry wallets, WalletKey walletKey) { }
    }
}
