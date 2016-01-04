using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Global Installer")]
  public class GlobalInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindAllInterfacesToSingle<Bootstrapper>();

      InstallServiceBindings();
    }

    private void InstallServiceBindings()
    {
      Container.Bind<IEventAggregator>().ToSingle<EventAggregator>();
      Container.Bind<SaveService>().ToSingle();
      Container.Bind<PlayerStatsService>().ToSingle();
    }
  }
}