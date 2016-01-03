namespace PachowStudios.BadTummyBunny
{
  public interface IScoreKeeper
  {
    int Coins { get; }

    void AddCoins(int coins);
    void RemoveCoins(int coins);
  }

}