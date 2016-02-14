using System;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class Bootystrapper : IInitializable, IDisposable
  {
    [Inject] private SaveService SaveService { get; set; }
    [Inject] private PlayerStatsService PlayerStatsService { get; set; }
    [Inject] private IScoreKeeper PlayerScoreService { get; set; }

    public void Initialize()
    {
      SaveService.Load();
    }

    public void Dispose()
    {
      SaveService.Save();
    }
  }
}
