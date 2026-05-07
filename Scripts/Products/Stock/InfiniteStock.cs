using System;

namespace KodachiGames.Economy
{
    [SelectorName("Stock/Infinite")]
    [Serializable]
    public class InfiniteStock : IStock
    {
        public bool IsAvailable => true;
        public void Consume() { }
    }
}
