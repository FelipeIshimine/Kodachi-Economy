using UnityEngine;

namespace KodachiGames.Economy
{
    [CreateAssetMenu(menuName = "Kodachi/Economy/Currency Definition", order = 2)]
    public class CurrencyDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
    }
}
