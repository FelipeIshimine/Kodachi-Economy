using UnityEngine;

namespace KodachiGames.Economy.Samples
{
    // Minimal single-entry registry stubs for use in samples (no ServiceLocator, no persistence).

    internal sealed class StubWalletRegistry : IWalletRegistry
    {
        private readonly WalletKey _key;
        private readonly CurrencyWallet _wallet;

        public StubWalletRegistry(WalletKey key, CurrencyWallet wallet)
        {
            _key    = key;
            _wallet = wallet;
        }

        public void Register(WalletKey key) { }
        public void Unregister(WalletKey key) { }
        public CurrencyWallet Get(WalletKey key) => _wallet;
        public bool TryGet(WalletKey key, out CurrencyWallet wallet) { wallet = _wallet; return true; }
    }

    internal sealed class StubInventoryRegistry : IInventoryRegistry
    {
        private readonly InventoryKey _key;
        private readonly Inventory _inventory;

        public StubInventoryRegistry(InventoryKey key, Inventory inventory)
        {
            _key       = key;
            _inventory = inventory;
        }

        public void Register(InventoryKey key) { }
        public void Unregister(InventoryKey key) { }
        public Inventory Get(InventoryKey key) => _inventory;
        public bool TryGet(InventoryKey key, out Inventory inventory) { inventory = _inventory; return true; }
    }
}
