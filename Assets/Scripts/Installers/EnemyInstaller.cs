using System;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Enemy Installer")]
  public class EnemyInstaller : MonoInstaller
  {
    [SerializeField] private EnemyFactory.Settings enemySettings;

    public override void InstallBindings()
    {
      Container.BindInstance(this.enemySettings);

      Container.BindFacadeFactory<Enemy.Settings, EnemyView, Enemy, EnemyFacadeFactory>(InstallFacade);
      Container.BindIFactory<EnemyType, Enemy>().ToCustomFactory<EnemyFactory>();
      Container.BindIFactory<EnemyView, Enemy>().ToCustomFactory<EnemyFactory>();
      Container.Bind<Enemy>().ToIFactoryWithContext<EnemyView, Enemy>();
    }

    private static void InstallFacade(DiContainer subContainer, Enemy.Settings settings, EnemyView view)
    {
      subContainer.BindInstance(settings);
      subContainer.BindSingleWithInterfaces(view.Type.GetTypeMapping());
      subContainer.BindInstance(view);
    }
  }
}