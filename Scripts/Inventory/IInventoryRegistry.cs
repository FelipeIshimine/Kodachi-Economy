namespace KodachiGames.Economy
{
    public interface IInventoryRegistry
    {
        void Register(InventoryKey key);
        void Unregister(InventoryKey key);
        Inventory Get(InventoryKey key);
        bool TryGet(InventoryKey key, out Inventory inventory);
    }
}
