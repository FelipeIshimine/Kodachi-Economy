using System.Collections.Generic;
using System.Threading;
using KodachiGames.Persistence;
using UnityEngine;

namespace KodachiGames.Economy
{
    public class WalletRegistry : IWalletRegistry
    {
        private readonly Dictionary<WalletKey, CurrencyWallet> _wallets = new();
        private readonly IPersistenceBackend _persistence;

        public WalletRegistry(IPersistenceBackend persistence) => _persistence = persistence;

        public void Register(WalletKey key)
        {
            Debug.Assert(!_wallets.ContainsKey(key), $"Wallet already registered for key '{key.name}'");
            _wallets[key] = new CurrencyWallet();
        }

        public void Unregister(WalletKey key)
        {
            Debug.Assert(_wallets.ContainsKey(key), $"No wallet registered for key '{key.name}'");
            _wallets.Remove(key);
        }

        public CurrencyWallet Get(WalletKey key)
        {
            Debug.Assert(_wallets.ContainsKey(key), $"No wallet registered for key '{key.name}'");
            return _wallets[key];
        }

        public bool TryGet(WalletKey key, out CurrencyWallet wallet) =>
            _wallets.TryGetValue(key, out wallet);

        internal async Awaitable LoadAsync(WalletKey key, CancellationToken ct)
        {
            var persistenceKey = PersistenceKey(key);
            if (!await _persistence.ExistsAsync(persistenceKey, ct)) return;

            var data = await _persistence.LoadAsync<WalletSaveData>(persistenceKey, ct);
            _wallets[key].LoadFromSaveData(data);
        }

        internal async Awaitable SaveAsync(WalletKey key, CancellationToken ct)
        {
            await _persistence.SaveAsync(PersistenceKey(key), _wallets[key].ToSaveData(), ct);
        }

        private static string PersistenceKey(WalletKey key) => $"wallet/{key.name}";
    }
}
