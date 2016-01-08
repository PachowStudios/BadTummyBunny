using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Enemy Installer")]
  public class EnemyInstaller : MonoInstaller
  {
    [SerializeField] private List<Enemy.Settings> enemySettings = null;

    public override void InstallBindings()
    {
      Container.BindFacadeFactory<Enemy.Settings, EnemyView, Enemy, EnemyFacadeFactory>(InstallFacade);

      Container.BindIFactory<EnemyType, Enemy>().ToCustomFactory<EnemyFactory>();
      Container.BindIFactory<EnemyView, Enemy>().ToCustomFactory<EnemyFactory>();
      Container.BindInstance(this.enemySettings).WhenInjectedInto<EnemyFactory>();

      Container.Bind<Enemy>().ToIFactoryWithContext<EnemyView, Enemy>();
    }

    private static void InstallFacade(DiContainer subContainer, Enemy.Settings settings, EnemyView view)
    {
      subContainer.Bind<IEventAggregator>().ToSingle<EventAggregator>();

      subContainer.BindInstance(settings);
      subContainer.BindSingleWithInterfaces(view.Type.GetTypeMapping());
      subContainer.BindInstance(view);
    }
  }
}
