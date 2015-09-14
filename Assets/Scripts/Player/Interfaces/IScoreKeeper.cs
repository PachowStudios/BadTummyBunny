using System;

public interface IScoreKeeper
{
	event Action<int> CoinsChanged;

	int Coins { get; }

	void AddCoins(int coins);
	void RemoveCoins(int coins);
}