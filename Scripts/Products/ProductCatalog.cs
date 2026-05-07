using UnityEngine;

namespace KodachiGames.Economy
{
    [CreateAssetMenu(menuName = "Kodachi/Economy/Product Catalog", order = 4)]
    public class ProductCatalog : ScriptableObject
    {
        [SerializeField] private ProductDefinition[] products;

        public ProductDefinition[] Products => products;
    }
}
