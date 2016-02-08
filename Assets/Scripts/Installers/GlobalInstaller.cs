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

      InstallGeneralServiceBindings();
      InstallPlayerServiceBindings();
    }

    private void InstallGeneralServiceBindings()
    {
      Container.Bind<IEventAggregator>(BindingIds.Global).ToSingle<EventAggregator>();
      Container.Bind<IEventAggregator>().ToLookup<IEventAggregator>(BindingIds.Global);

      Container.BindSingleWithInterfaces<SceneService>();
      Container.BindSingle<SaveService>();
    }

    private void InstallPlayerServiceBindings()
    {
      Container.BindSingleWithInterfaces<PlayerScoreService>();
      Container.BindSingle<PlayerStatsService>();
    }
  }
}