using System.Reflection;
using UnityEngine;

namespace KodachiGames.Economy.Samples
{
    internal static class SampleDataBuilder
    {
        internal static CurrencyDefinition BuildCurrency(string id, string displayName)
        {
            var cur = ScriptableObject.CreateInstance<CurrencyDefinition>();
            SetField(cur, "id", id);
            SetField(cur, "displayName", displayName);
            return cur;
        }

        internal static ProductDefinition BuildFreeProduct(string id, string displayName)
        {
            var product = ScriptableObject.CreateInstance<ProductDefinition>();
            SetField(product, "id", id);
            SetField(product, "displayName", displayName);
            SetField(product, "stock", new InfiniteStock());
            SetField(product, "requirements", new IAcquisitionRequirement[] { new FreeRequirement() });
            return product;
        }

        internal static ProductDefinition BuildCostProduct(string id, string displayName, CurrencyDefinition currency, int amount)
        {
            var req = new CurrencyCostRequirement();
            SetField(req, "currency", currency);
            SetField(req, "amount", amount);

            var product = ScriptableObject.CreateInstance<ProductDefinition>();
            SetField(product, "id", id);
            SetField(product, "displayName", displayName);
            SetField(product, "stock", new InfiniteStock());
            SetField(product, "requirements", new IAcquisitionRequirement[] { req });
            return product;
        }

        internal static ProductDefinition BuildLimitedProduct(string id, string displayName, CurrencyDefinition currency, int amount, int stockQuantity)
        {
            var req = new CurrencyCostRequirement();
            SetField(req, "currency", currency);
            SetField(req, "amount", amount);

            var stock = new FiniteStock();
            SetField(stock, "quantity", stockQuantity);

            var product = ScriptableObject.CreateInstance<ProductDefinition>();
            SetField(product, "id", id);
            SetField(product, "displayName", displayName);
            SetField(product, "stock", stock);
            SetField(product, "requirements", new IAcquisitionRequirement[] { req });
            return product;
        }

        private static void SetField(object target, string fieldName, object value) =>
            target.GetType()
                  .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                  ?.SetValue(target, value);
    }
}
