# Kodachi Economy

Keyed wallet, inventory, product catalog, and shop systems for Kodachi Games projects. All state is identified by ScriptableObject keys — no singletons, no magic strings.

## Key Concepts

| Type | Role |
|---|---|
| `WalletKey` / `InventoryKey` | ScriptableObject identity tokens. Dragged into Inspector fields via the key drawer. |
| `CurrencyWallet` | Tracks balances per `CurrencyDefinition`. Fires `OnBalanceChanged`. |
| `Inventory` | Tracks owned `ProductDefinition` IDs. Fires `OnProductGranted` / `OnProductRevoked`. |
| `IWalletRegistry` / `IInventoryRegistry` | Look up wallets/inventories by key. Registered to ServiceLocator. |
| `ProductDefinition` | ScriptableObject: id, name, icon, `IStock`, `IAcquisitionRequirement[]`. |
| `ProductCatalog` | ScriptableObject array of products. Drives shop UI. |
| `ShopService` | Pure transaction engine. Call `TryPurchase` — returns a `PurchaseResult`. |

## Quick Start

### 1. Create Keys

In any key field, use the **Select ▾** or **+ Create** buttons in the Inspector — never create assets manually.

### 2. Scene Setup

Add these components to your global ServiceLocator GameObject (in this order of execution):

```
1. PersistenceServiceInstaller   (from KodachiPersistence)
2. WalletRegistryInstaller        — assign WalletKey assets
3. InventoryRegistryInstaller     — assign InventoryKey assets
4. ShopServiceInstaller
```

> **Execution order matters.** `WalletRegistryInstaller.Awake` resolves `IPersistenceBackend`, so `PersistenceServiceInstaller` must register first. Set Script Execution Order in Project Settings if needed.

### 3. Create Data Assets

- `Kodachi/Economy/Currency Definition` — set a stable `Id` string
- `Kodachi/Economy/Product Definition` — set a stable `Id`, choose Stock and Requirements via TypeSelector
- `Kodachi/Economy/Product Catalog` — reference your products

### 4. Use at Runtime

```csharp
// Resolve
ServiceLocator.For(this).Get<IWalletRegistry>(out var wallets);
ServiceLocator.For(this).Get<IInventoryRegistry>(out var inventories);
ServiceLocator.For(this).Get<ShopService>(out var shop);

// Credit currency
wallets.Get(playerWalletKey).Credit(coinsCurrency, 100);

// Purchase
var result = shop.TryPurchase(swordProduct, playerWalletKey, playerInventoryKey);
switch (result)
{
    case PurchaseSuccess:         // grant succeeded
    case PurchaseNotAffordable n: // n.FailedRequirements
    case PurchaseOutOfStock:      // finite stock depleted
    case PurchaseAlreadyOwned:    // product already in inventory
}
```

## Extension Points

### Custom Stock

```csharp
[SelectorName("Stock/My Custom Stock")]
[System.Serializable]
public class MyStock : IStock
{
    public bool IsAvailable => /* your logic */;
    public void Consume() { /* your logic */ }
}
```

### Custom Requirement

```csharp
[SelectorName("Requirements/Level Required")]
[System.Serializable]
public class LevelRequirement : IAcquisitionRequirement
{
    [SerializeField] private int requiredLevel;

    public bool IsMet(IWalletRegistry wallets, WalletKey walletKey) => /* check level */;
    public void Fulfill(IWalletRegistry wallets, WalletKey walletKey) { }
}
```

## Important Notes

- **`CurrencyDefinition.Id` and `ProductDefinition.Id`** must be set to stable, unique strings. Save data is keyed on these. Changing them after shipping breaks existing saves.
- **`FiniteStock`** resets per session — `_consumed` is runtime-only state, not persisted.
- **`WalletRegistryInstaller.OnDestroy`** saves all wallets with `CancellationToken.None`. On app quit, ensure the process has time to complete (PlayerPrefs backend is synchronous; binary file uses a background thread).

## Dependencies

- `com.kodachigames.persistence` — `IPersistenceBackend` for save/load
- `com.unity.nuget.newtonsoft-json` — JSON serialization
