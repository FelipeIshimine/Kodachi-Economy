using System.Collections.Generic;

namespace KodachiGames.Economy
{
    public class ShopService
    {
        private readonly IWalletRegistry _wallets;
        private readonly IInventoryRegistry _inventories;

        public ShopService(IWalletRegistry wallets, IInventoryRegistry inventories)
        {
            _wallets = wallets;
            _inventories = inventories;
        }

        public PurchaseResult TryPurchase(ProductDefinition product, WalletKey walletKey, InventoryKey inventoryKey)
        {
            var inventory = _inventories.Get(inventoryKey);

            if (!product.Stackable && inventory.HasProduct(product))
                return new PurchaseAlreadyOwned();

            if (!product.Stock.IsAvailable)
                return new PurchaseOutOfStock();

            var failedRequirements = new List<IAcquisitionRequirement>();
            foreach (var requirement in product.Requirements)
            {
                if (!requirement.IsMet(_wallets, walletKey))
                    failedRequirements.Add(requirement);
            }

            if (failedRequirements.Count > 0)
                return new PurchaseNotAffordable(failedRequirements);

            foreach (var requirement in product.Requirements)
                requirement.Fulfill(_wallets, walletKey);
            product.Stock.Consume();
            inventory.Grant(product);

            return new PurchaseSuccess();
        }
    }
}
