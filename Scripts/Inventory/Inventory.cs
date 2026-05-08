using System;
using System.Collections.Generic;

namespace KodachiGames.Economy
{
    public class Inventory
    {
        private readonly Dictionary<string, int> _productCounts = new();

        public event Action<ProductDefinition> OnProductGranted;
        public event Action<ProductDefinition> OnProductRevoked;

        public bool HasProduct(ProductDefinition product) =>
            _productCounts.TryGetValue(product.Id, out var count) && count > 0;

        public int GetCount(ProductDefinition product) =>
            _productCounts.TryGetValue(product.Id, out var count) ? count : 0;

        public void Grant(ProductDefinition product, int quantity = 1)
        {
            _productCounts.TryGetValue(product.Id, out var current);
            _productCounts[product.Id] = current + quantity;
            OnProductGranted?.Invoke(product);
        }

        public void Revoke(ProductDefinition product, int quantity = 1)
        {
            if (!_productCounts.TryGetValue(product.Id, out var current)) return;
            var next = current - quantity;
            if (next <= 0)
                _productCounts.Remove(product.Id);
            else
                _productCounts[product.Id] = next;
            OnProductRevoked?.Invoke(product);
        }

        internal InventorySaveData ToSaveData()
        {
            var entries = new List<InventorySaveData.Entry>();
            foreach (var kvp in _productCounts)
                entries.Add(new InventorySaveData.Entry { ProductId = kvp.Key, Count = kvp.Value });
            return new InventorySaveData { Entries = entries };
        }

        internal void LoadFromSaveData(InventorySaveData data)
        {
            _productCounts.Clear();
            foreach (var entry in data.Entries)
                _productCounts[entry.ProductId] = entry.Count;
        }
    }
}
