using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Enemy Installer")]
  public class EnemyInstaller : MonoInstaller
  {
    [SerializeField] private List<EnemySettings> enemySettings = null;

    public override void InstallBindings()
    {
      Container.BindInstance(this.enemySettings).WhenInjectedInto<EnemyFactory>();

      Container.BindFacadeFactory<EnemySettings, EnemyView, Enemy, EnemyFacadeFactory>(InstallFacade);

      Container.BindIFactory<EnemyType, Enemy>().ToCustomFactory<EnemyFactory>();
      Container.BindIFactory<EnemyView, Enemy>().ToCustomFactory<EnemyFactory>();

      Container.Bind<Enemy>().ToIFactoryWithContext<EnemyView, Enemy>();

      var test = Container.InstantiatePrefab(this.enemySettings.First().Prefab);

      Debug.Log(test.Model.Name);
    }

    private static void InstallFacade(DiContainer subContainer, EnemySettings settings, EnemyView view)
    {
      subContainer.Bind<IEventAggregator>().ToSingle<EventAggregator>();

      subContainer.BindInstance(settings);
      subContainer.BindAbstractInstance(settings.Movement);
      subContainer.BindAbstractInstance(settings.Health);

      subContainer.BindSingleWithInterfaces(view.Type.GetTypeMapping());
      subContainer.BindInstance(view);
    }
  }
}
