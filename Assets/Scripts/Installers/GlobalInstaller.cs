using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Global Installer")]
  public class GlobalInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindAllInterfacesToSingle<Bootystrapper>();

      InstallServiceBindings();
    }

    private void InstallServiceBindings()
    {
      Container.Bind<IEventAggregator>().ToSingle<EventAggregator>();
      Container.Bind<IEventAggregator>(BindingIds.Global).ToSingle<EventAggregator>();
      Container.BindSingleWithInterfaces<SceneService>();
      Container.BindSingle<SaveService>();
      Container.BindSingle<PlayerStatsService>();
    }
  }
}