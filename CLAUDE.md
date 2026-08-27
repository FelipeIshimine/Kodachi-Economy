# KodachiEconomy CLAUDE.md

**KodachiEconomy** is a keyed wallet, inventory, product catalog, and shop system for Kodachi Games projects. All state is identified by **ScriptableObject keys** — no singletons, no magic strings, no global databases.

## Package Overview

### Core Systems

**1. Wallets & Currency**
- `CurrencyWallet` — Tracks currency balances; fires `OnBalanceChanged` event
- `IWalletRegistry` — Registry mapping `WalletKey` → `CurrencyWallet`
- `WalletRegistryInstaller` — Registers wallets to ServiceLocator in Awake
- `CurrencyDefinition` — ScriptableObject defining a currency (coins, gems, etc.)

**2. Inventory & Products**
- `Inventory` — Tracks owned product IDs; fires `OnProductGranted` / `OnProductRevoked`
- `IInventoryRegistry` — Registry mapping `InventoryKey` → `Inventory`
- `InventoryRegistryInstaller` — Registers inventories to ServiceLocator
- `ProductDefinition` — ScriptableObject: ID, name, icon, stock rules, acquisition requirements

**3. Shop & Transactions**
- `ProductCatalog` — ScriptableObject array of products for shop display
- `ShopService` — Stateless transaction engine; call `TryPurchase()` → `PurchaseResult`
- `PurchaseResult` — Base type for `PurchaseSuccess`, `PurchaseNotAffordable`, `PurchaseOutOfStock`, `PurchaseAlreadyOwned`

**4. Extensibility**
- `IStock` — Define product availability (infinite, finite, limited per user)
- `IAcquisitionRequirement` — Define purchase conditions (currency cost, level requirement, etc.)

## Folder Structure

```
Packages/com.kodachigames.economy/
├── Scripts/
│   ├── Currency/
│   │   └── CurrencyDefinition.cs           # ScriptableObject: currency definition
│   ├── Keys/
│   │   ├── WalletKey.cs                    # ScriptableObject identity token
│   │   └── InventoryKey.cs                 # ScriptableObject identity token
│   ├── Wallet/
│   │   ├── CurrencyWallet.cs               # Runtime wallet state
│   │   ├── IWalletRegistry.cs              # Wallet lookup interface
│   │   ├── WalletRegistry.cs               # Registry implementation
│   │   ├── WalletSaveData.cs               # Serializable save state
│   │   └── WalletRegistryInstaller.cs      # ServiceLocator registration
│   ├── Inventory/
│   │   ├── Inventory.cs                    # Runtime inventory state
│   │   ├── IInventoryRegistry.cs           # Inventory lookup interface
│   │   ├── InventoryRegistry.cs            # Registry implementation
│   │   ├── InventorySaveData.cs            # Serializable save state
│   │   └── InventoryRegistryInstaller.cs   # ServiceLocator registration
│   ├── Products/
│   │   ├── ProductDefinition.cs            # ScriptableObject: product data
│   │   ├── ProductCatalog.cs               # ScriptableObject: product array
│   │   ├── Stock/                          # IStock implementations
│   │   │   └── FiniteStock.cs              # Limited per-session stock
│   │   └── Requirements/                   # IAcquisitionRequirement implementations
│   │       └── CurrencyRequirement.cs      # Currency cost requirement
│   ├── Shop/
│   │   ├── ShopService.cs                  # Transaction engine
│   │   └── PurchaseResult.cs               # Purchase outcome types
│   ├── Installers/
│   │   ├── WalletRegistryInstaller.cs      # (duplicate reference)
│   │   ├── InventoryRegistryInstaller.cs   # (duplicate reference)
│   │   └── ShopServiceInstaller.cs         # ShopService registration
│   ├── Editor/
│   │   ├── EconomyEditorSettings.cs        # Editor preferences
│   │   └── Keys/                           # Key creation helpers
│   ├── KodachiGames.Economy.asmdef         # Runtime assembly
│   ├── KodachiGames.Economy.Editor.asmdef  # Editor assembly
│   └── AssemblyInfo.cs
├── Samples~/
├── package.json
└── README.md
```

## Key Design Principles

### 1. Keys as Identity, Not Names

Use **ScriptableObject keys** instead of strings or enum values:

```csharp
[SerializeField] private WalletKey _playerWalletKey;  // Drag-and-drop in Inspector
[SerializeField] private InventoryKey _playerInventoryKey;

// Never use magic strings:
// ❌ wallets.Get("player-main")
// ✓ wallets.Get(_playerWalletKey)
```

Keys are created via Inspector button (Select ▾ or + Create) — never manually create assets.

### 2. Registries Over Singletons

Wallets and inventories live in registries, resolved from ServiceLocator:

```csharp
ServiceLocator.For(this).Get<IWalletRegistry>(out var wallets);
var playerWallet = wallets.Get(_playerWalletKey);
```

### 3. Stateless Transactions

`ShopService.TryPurchase()` is a pure function — no side effects beyond the wallets/inventories passed:

```csharp
var result = shopService.TryPurchase(
    productDefinition,
    _playerWalletKey,
    _playerInventoryKey
);

// Check outcome
if (result is PurchaseSuccess success) { /* ... */ }
else if (result is PurchaseNotAffordable fail) { /* handle */ }
```

### 4. Save Data Separation

Runtime state (`CurrencyWallet`, `Inventory`) is distinct from save data (`WalletSaveData`, `InventorySaveData`). Installers serialize/deserialize on init.

## Scene Setup

Add these components to your global **ServiceLocator** GameObject, **in order**:

### 1. PersistenceServiceInstaller
From `KodachiPersistence`; must run first to register `IPersistenceBackend`.

### 2. WalletRegistryInstaller
Assign your `WalletKey` assets in the Inspector. In `Awake`:
- Loads wallet data from persistence backend
- Creates `CurrencyWallet` instances
- Registers to `IWalletRegistry`

### 3. InventoryRegistryInstaller
Assign your `InventoryKey` assets in the Inspector. In `Awake`:
- Loads inventory data from persistence backend
- Creates `Inventory` instances
- Registers to `IInventoryRegistry`

### 4. ShopServiceInstaller
Assigns `ProductCatalog` and registers `ShopService` to ServiceLocator.

```
ServiceLocator (root)
├─ PersistenceServiceInstaller
├─ WalletRegistryInstaller (drag WalletKey assets here)
├─ InventoryRegistryInstaller (drag InventoryKey assets here)
└─ ShopServiceInstaller (assign ProductCatalog)
```

## Creating Data Assets

### CurrencyDefinition
Create via **Assets > Create > Kodachi/Economy/Currency Definition**:
- **Id**: Stable unique string (e.g., `"coins"`, `"gems"`). Never change after shipping.
- **Display Name**: Human-readable name for UI
- **Icon**: Sprite for UI display

### ProductDefinition
Create via **Assets > Create > Kodachi/Economy/Product Definition**:
- **Id**: Stable unique string (e.g., `"sword-iron"`, `"potion-health"`). Never change after shipping.
- **Display Name**: Product name for shop UI
- **Icon**: Product artwork
- **Stackable**: Checkbox (default false)
  - `true` — Can purchase multiple times; no "already owned" check
  - `false` — Single-purchase item; second purchase returns `PurchaseAlreadyOwned`
- **Stock**: Use TypeSelector to choose `IStock` implementation
  - `Infinite` — Always available
  - `Finite` — Limited per session (runtime-only)
- **Requirements**: Array of `IAcquisitionRequirement`
  - `Currency Requirement` — Deduct currency on purchase
  - Add custom requirements via TypeSelector

### ProductCatalog
Create via **Assets > Create > Kodachi/Economy/Product Catalog**:
- **Products**: Drag your `ProductDefinition` assets here

### Keys (WalletKey, InventoryKey)
**Never create manually.** Use Inspector buttons:
1. In any `WalletKey` or `InventoryKey` field, click **Select ▾**
2. Click **+ Create** → choose location
3. The Inspector saves the reference for you

## Runtime Usage

### Resolve Services

```csharp
ServiceLocator services = ServiceLocator.For(this);
services.Get<IWalletRegistry>(out var wallets);
services.Get<IInventoryRegistry>(out var inventories);
services.Get<ShopService>(out var shop);
```

### Credit & Spend Currency

```csharp
var playerWallet = wallets.Get(_playerWalletKey);

// Add currency (always succeeds)
playerWallet.Credit(_coinsCurrency, 100);  // Add 100 coins

// Remove currency (check balance first)
if (playerWallet.TrySpend(_coinsCurrency, 50))
{
    Debug.Log("Spent 50 coins");
}
else
{
    Debug.Log("Not enough coins");
}

// Get current balance
int balance = playerWallet.GetBalance(_coinsCurrency);
```

**Methods:**
- `Credit(currency, amount)` — Add currency (always succeeds, fires `OnBalanceChanged`)
- `TrySpend(currency, amount)` — Attempt to remove currency (returns bool, fires event only if successful)
- `GetBalance(currency)` — Query current balance (returns 0 if currency not in wallet)

### Listen for Balance Changes

```csharp
var playerWallet = wallets.Get(_playerWalletKey);
playerWallet.OnBalanceChanged += (currency, newBalance) =>
{
    _balanceUI.SetText($"{newBalance} {currency.DisplayName}");
};
```

**Methods:**
- `Credit(currency, amount)` — Add currency (always succeeds, fires `OnBalanceChanged`)
- `TrySpend(currency, amount)` — Attempt to remove currency (returns bool, fires event only if successful)
- `GetBalance(currency)` — Query current balance (returns 0 if currency not in wallet)

### Purchase Products

```csharp
var result = shop.TryPurchase(swordProduct, _playerWalletKey, _playerInventoryKey);

if (result is PurchaseSuccess success)
{
    // Product added to inventory, currency deducted
    ShowPurchaseConfirmation("Sword purchased!");
}
else if (result is PurchaseNotAffordable failed)
{
    // Check which requirements failed
    foreach (var req in failed.FailedRequirements)
    {
        Debug.Log($"Missing: {req}");
    }
}
else if (result is PurchaseOutOfStock)
{
    ShowError("Out of stock");
}
else if (result is PurchaseAlreadyOwned)
{
    ShowError("Already owned");
}
```

### Check Inventory

```csharp
var playerInventory = inventories.Get(_playerInventoryKey);
if (playerInventory.Contains(swordProduct.Id))
{
    ShowEquipButton();
}
```

### Listen for Inventory Changes

```csharp
var playerInventory = inventories.Get(_playerInventoryKey);
playerInventory.OnProductGranted += (productId) =>
{
    Debug.Log($"Granted: {productId}");
};
playerInventory.OnProductRevoked += (productId) =>
{
    Debug.Log($"Revoked: {productId}");
};
```

## Extension Points

### Custom Stock Type

Define availability rules per product:

```csharp
[SelectorName("Stock/Level-Gated")]
[System.Serializable]
public class LevelGatedStock : IStock
{
    [SerializeField] private int _requiredLevel;

    public bool IsAvailable => GameManager.Instance.PlayerLevel >= _requiredLevel;
    public void Consume() { /* no-op for level-gated items */ }
}
```

### Custom Acquisition Requirement

Define purchase conditions:

```csharp
[SelectorName("Requirements/Level Required")]
[System.Serializable]
public class LevelRequirement : IAcquisitionRequirement
{
    [SerializeField] private int _requiredLevel;

    public bool IsMet(IWalletRegistry wallets, WalletKey walletKey)
    {
        return GameManager.Instance.PlayerLevel >= _requiredLevel;
    }

    public void Fulfill(IWalletRegistry wallets, WalletKey walletKey)
    {
        // No fulfillment needed (not a cost)
    }
}
```

Use TypeSelector on `ProductDefinition.Requirements[]` to add custom types.

## Important Notes

- **CurrencyDefinition.Id and ProductDefinition.Id** are immutable save keys. Changing them after shipping **breaks existing saves**. Plan stable IDs early.
- **Stackable Products**: Non-stackable products (`Stackable = false`) enforce single ownership via inventory; stackable ones allow duplicates.
- **FiniteStock** only persists availability within a session. On app restart, stock is reset. Use a custom `IStock` for persistent availability.
- **TrySpend vs Debit**: Use `TrySpend()` when spending might fail (player affordability check); it returns false without firing `OnBalanceChanged`. Direct negative `Credit()` is valid but doesn't provide feedback.
- **WalletRegistryInstaller.OnDestroy** saves all wallets synchronously. Ensure the app has time to quit gracefully.
- **Wallet/Inventory events** fire immediately on state changes; listen in `Start` or later, not in `Awake`.
- **ProductCatalog** is read-only at runtime; define products before shipping.
- **Purchase Transaction Atomicity**: `ShopService.TryPurchase()` is atomic — either all requirements pass and all state updates complete, or none occur.

## Dependencies

- **KodachiPersistence** — `IPersistenceBackend` for save/load wallet and inventory state
- **Unity-Service-Locator** — Resolve wallets, inventories, and shop service
- **Unity `JsonUtility`** — JSON serialization (via `IPersistenceBackend`; no external dependency)

## Tooling

Use the Rider MCP server for file operations. Prefer `mcp__rider__read_file`, `mcp__rider__search_symbol`, and `mcp__rider__get_symbol_info` for code navigation.
