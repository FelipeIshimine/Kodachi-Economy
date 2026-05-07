using KodachiGames.Persistence;
using UnityEngine;
using UnityServiceLocator;

namespace KodachiGames.Economy
{
    public class WalletRegistryInstaller : MonoBehaviour
    {
        [SerializeField] private WalletKey[] walletsToRegister;

        private WalletRegistry _registry;

        private void Awake()
        {
            ServiceLocator.For(this).Get<IPersistenceBackend>(out var persistence);
            _registry = new WalletRegistry(persistence);

            foreach (var key in walletsToRegister)
                _registry.Register(key);

            ServiceLocator.For(this).Register<IWalletRegistry>(_registry);
        }

        private async void Start()
        {
            foreach (var key in walletsToRegister)
                await _registry.LoadAsync(key, destroyCancellationToken);
        }

        private async void OnDestroy()
        {
            foreach (var key in walletsToRegister)
                await _registry.SaveAsync(key, default);

            ServiceLocator.For(this).Unregister<IWalletRegistry>(_registry);
        }
    }
}
