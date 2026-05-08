using System.Collections.Generic;
using UnityEngine;

namespace KodachiGames.Economy
{
    [CreateAssetMenu(menuName = "Kodachi/Economy/Product Definition", order = 3)]
    public class ProductDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private bool stackable;
        [SerializeReference, TypeSelector] private IStock stock;
        [SerializeReference, TypeSelector] private IAcquisitionRequirement[] requirements;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public bool Stackable => stackable;
        public IStock Stock => stock;
        public IReadOnlyList<IAcquisitionRequirement> Requirements => requirements;
    }
}
