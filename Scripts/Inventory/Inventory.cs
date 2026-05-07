using System;
using System.Collections.Generic;

namespace KodachiGames.Economy
{
    public class Inventory
    {
        private readonly HashSet<string> _ownedProductIds = new();

        public event Action<ProductDefinition> OnProductGranted;
        public event Action<ProductDefinition> OnProductRevoked;

        public bool HasProduct(ProductDefinition product) =>
            _ownedProductIds.Contains(product.Id);

        public void Grant(ProductDefinition product)
        {
            _ownedProductIds.Add(product.Id);
            OnProductGranted?.Invoke(product);
        }

        public void Revoke(ProductDefinition product)
        {
            _ownedProductIds.Remove(product.Id);
            OnProductRevoked?.Invoke(product);
        }

        internal InventorySaveData ToSaveData() => new()
        {
            OwnedProductIds = new List<string>(_ownedProductIds)
        };

        internal void LoadFromSaveData(InventorySaveData data)
        {
            _ownedProductIds.Clear();
            foreach (var id in data.OwnedProductIds)
                _ownedProductIds.Add(id);
        }
    }
}
