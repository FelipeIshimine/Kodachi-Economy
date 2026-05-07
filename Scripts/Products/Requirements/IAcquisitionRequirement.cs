namespace KodachiGames.Economy
{
    public interface IAcquisitionRequirement
    {
        bool IsMet(IWalletRegistry wallets, WalletKey walletKey);
        void Fulfill(IWalletRegistry wallets, WalletKey walletKey);
    }
}
