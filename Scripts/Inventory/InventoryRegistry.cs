using System.Collections.Generic;
using System.Threading;
using KodachiGames.Persistence;
using UnityEngine;

namespace KodachiGames.Economy
{
    public class InventoryRegistry : IInventoryRegistry
    {
        private readonly Dictionary<InventoryKey, Inventory> _inventories = new();
        private readonly IPersistenceBackend _persistence;

        public InventoryRegistry(IPersistenceBackend persistence) => _persistence = persistence;

        public void Register(InventoryKey key)
        {
            Debug.Assert(!_inventories.ContainsKey(key), $"Inventory already registered for key '{key.name}'");
            _inventories[key] = new Inventory();
        }

        public void Unregister(InventoryKey key)
        {
            Debug.Assert(_inventories.ContainsKey(key), $"No inventory registered for key '{key.name}'");
            _inventories.Remove(key);
        }

        public Inventory Get(InventoryKey key)
        {
            Debug.Assert(_inventories.ContainsKey(key), $"No inventory registered for key '{key.name}'");
            return _inventories[key];
        }

        public bool TryGet(InventoryKey key, out Inventory inventory) =>
            _inventories.TryGetValue(key, out inventory);

        internal async Awaitable LoadAsync(InventoryKey key, CancellationToken ct)
        {
            var persistenceKey = PersistenceKey(key);
            if (!_persistence.Exists(persistenceKey)) return;

            var data = await _persistence.LoadAsync<InventorySaveData>(persistenceKey, ct);
            _inventories[key].LoadFromSaveData(data);
        }

        internal async Awaitable SaveAsync(InventoryKey key, CancellationToken ct)
        {
            await _persistence.SaveAsync(PersistenceKey(key), _inventories[key].ToSaveData(), ct);
        }

        private static string PersistenceKey(InventoryKey key) => $"inventory/{key.name}";
    }
}
