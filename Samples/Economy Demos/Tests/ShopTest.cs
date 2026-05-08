using System;
using KodachiGames.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace KodachiGames.Economy.Samples
{
    [Serializable]
    [SelectorName("Economy/Shop — Purchases")]
    public class ShopTest : EconomyTestCase
    {
        public override void Build(VisualElement root)
        {
            root.SetPadding(24);
            root.style.flexDirection = FlexDirection.Column;
            root.Add(SampleUI.H1("Shop & Purchases"));
            root.Add(SampleUI.Body("Buy products with currency cost, free items, finite stock. See purchase outcomes."));

            // Currency + wallet
            var gold      = SampleDataBuilder.BuildCurrency("gold", "Gold");
            var wallet    = new CurrencyWallet();
            wallet.Credit(gold, 1000);

            // Keys (identity-only SOs for the stub registries)
            var walletKey    = ScriptableObject.CreateInstance<WalletKey>();
            var inventoryKey = ScriptableObject.CreateInstance<InventoryKey>();

            var inventory    = new Inventory();
            var walletReg    = new StubWalletRegistry(walletKey, wallet);
            var inventoryReg = new StubInventoryRegistry(inventoryKey, inventory);
            var shop         = new ShopService(walletReg, inventoryReg);

            var goldLabel = SampleUI.StatusLabel("Gold: 1000");
            wallet.OnBalanceChanged += (_, balance) => goldLabel.text = $"Gold: {balance}";

            var logBox = SampleUI.LogBox(out var log);

            // Products
            var freeItem      = SampleDataBuilder.BuildFreeProduct("item.free",       "Free Item");
            var costItem      = SampleDataBuilder.BuildCostProduct("item.expensive",  "Expensive Widget", gold, 300);
            var limitedItem   = SampleDataBuilder.BuildLimitedProduct("item.limited", "Limited Edition",  gold, 150, 2);
            var veryExpensive = SampleDataBuilder.BuildCostProduct("item.ultrarare",  "Ultra Rare",       gold, 2000);

            var controls = SampleUI.Row();

            controls.Add(SampleUI.Button("Buy Free Item", () =>
                Log(shop.TryPurchase(freeItem, walletKey, inventoryKey), "Free Item", log)));

            controls.Add(SampleUI.Button("Buy Expensive (300G)", () =>
                Log(shop.TryPurchase(costItem, walletKey, inventoryKey), "Expensive Widget", log)));

            controls.Add(SampleUI.Button("Buy Limited (150G, 2x)", () =>
                Log(shop.TryPurchase(limitedItem, walletKey, inventoryKey), "Limited Edition", log)));

            controls.Add(SampleUI.Button("Try Ultra Rare (2000G)", () =>
                Log(shop.TryPurchase(veryExpensive, walletKey, inventoryKey), "Ultra Rare", log)));

            root.Add(goldLabel);
            root.Add(logBox);
            root.Add(controls);
        }

        private static void Log(PurchaseResult result, string itemName, Action<string> log)
        {
            switch (result)
            {
                case PurchaseSuccess:
                    log($"✓ Purchased {itemName}");
                    break;
                case PurchaseNotAffordable notAffordable:
                    log($"✗ Can't afford {itemName} ({notAffordable.FailedRequirements.Count} unmet)");
                    break;
                case PurchaseOutOfStock:
                    log($"✗ {itemName} is out of stock");
                    break;
                case PurchaseAlreadyOwned:
                    log($"✗ Already own {itemName}");
                    break;
            }
        }
    }
}
