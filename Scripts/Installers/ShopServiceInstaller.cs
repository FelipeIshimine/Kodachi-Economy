using UnityEngine;
using UnityServiceLocator;

namespace KodachiGames.Economy
{
    public class ShopServiceInstaller : MonoBehaviour
    {
        private ShopService _service;

        private void Start()
        {
            ServiceLocator.For(this).Get<IWalletRegistry>(out var wallets);
            ServiceLocator.For(this).Get<IInventoryRegistry>(out var inventories);
            _service = new ShopService(wallets, inventories);
            ServiceLocator.For(this).Register<ShopService>(_service);
        }

        private void OnDestroy()
        {
            ServiceLocator.For(this).Unregister<ShopService>(_service);
        }
    }
}
