using System;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class Bootstrapper : IInitializable, IDisposable
  {
    [Inject]
    private SaveService SaveService { get; set; }

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
