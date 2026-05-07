namespace KodachiGames.Economy
{
    public interface IWalletRegistry
    {
        void Register(WalletKey key);
        void Unregister(WalletKey key);
        CurrencyWallet Get(WalletKey key);
        bool TryGet(WalletKey key, out CurrencyWallet wallet);
    }
}
