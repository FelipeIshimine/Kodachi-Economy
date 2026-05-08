using System;
using UnityEngine.UIElements;

namespace KodachiGames.Economy.Samples
{
    [Serializable]
    public abstract class EconomyTestCase
    {
        public abstract void Build(VisualElement root);
    }
}
