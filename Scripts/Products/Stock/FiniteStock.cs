using System;
using UnityEngine;

namespace KodachiGames.Economy
{
    // quantity is the configured max. _consumed is runtime-only and resets per session.
    [SelectorName("Stock/Finite")]
    [Serializable]
    public class FiniteStock : IStock
    {
        [SerializeField] private int quantity;

        private int _consumed;

        public bool IsAvailable => _consumed < quantity;

        public void Consume() => _consumed++;
    }
}
