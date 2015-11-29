using UnityEngine;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Startup/Global Installer")]
  public class GlobalInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      // Game management singletons
      Container.BindLifetimeSingleton<Bootstrapper>();

      // Services
      Container.Bind<IEventAggregator>().ToSingle<EventAggregator>();
      Container.Bind<SaveService>().ToSingle();
      Container.Bind<PlayerStatsService>().ToSingle();
    }
  }
}