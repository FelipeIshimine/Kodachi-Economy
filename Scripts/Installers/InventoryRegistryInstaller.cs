using KodachiGames.Persistence;
using UnityEngine;
using UnityServiceLocator;

namespace KodachiGames.Economy
{
    public class InventoryRegistryInstaller : MonoBehaviour
    {
        [SerializeField] private InventoryKey[] inventoriesToRegister;

        private InventoryRegistry _registry;

        private void Awake()
        {
            ServiceLocator.For(this).Get<IPersistenceBackend>(out var persistence);
            _registry = new InventoryRegistry(persistence);

            foreach (var key in inventoriesToRegister)
                _registry.Register(key);

            ServiceLocator.For(this).Register<IInventoryRegistry>(_registry);
        }

        private async void Start()
        {
            foreach (var key in inventoriesToRegister)
                await _registry.LoadAsync(key, destroyCancellationToken);
        }

        private async void OnDestroy()
        {
            foreach (var key in inventoriesToRegister)
                await _registry.SaveAsync(key, default);

            ServiceLocator.For(this).Unregister<IInventoryRegistry>(_registry);
        }
    }
}
