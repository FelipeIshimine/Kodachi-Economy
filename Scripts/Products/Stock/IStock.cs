namespace KodachiGames.Economy
{
    public interface IStock
    {
        bool IsAvailable { get; }
        void Consume();
    }
}
