using System;
using System.Collections.Generic;
using System.Linq;
using PachowStudios.Collections;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Installers/Enemy Installer")]
  public class EnemyInstaller : ExtendedFacadeMonoInstaller<EnemyView, Enemy>
  {
    [SerializeField] private List<EnemySettings> enemySettings = null;

    private IReadOnlyDictionary<EnemyType, EnemySettings> MappedSettings { get; set; }

    public override void InstallBindings()
    {
      MappedSettings = this.enemySettings.ToDictionary(s => s.Prefab.Type).AsReadOnly();

      Container.BindInstance(MappedSettings);
      Container.BindIFactory<EnemyType, Enemy>().ToCustomFactory<EnemyFactory>();

      BindToSubContainer<Enemy>();
      BindToSubContainer<IEventAggregator>();
    }

    protected override void InstallSubContainerBindings(DiContainer subContainer, EnemyView instance)
    {
      var settings = MappedSettings[instance.Type];

      subContainer.BindInstanceWithInterfaces(instance);

      subContainer.BindBaseInstance(settings);
      subContainer.BindBaseInstance(settings.Movement);
      subContainer.BindBaseInstance(settings.Health);

      var movementType = instance.Type.GetTypeMapping();
      subContainer.BindSingleWithInterfaces(movementType);
      subContainer.Bind<EnemyMovement>().ToSingle(movementType);

      subContainer.BindSingleWithInterfaces<EnemyHealth>();

      subContainer.Bind<IEventAggregator>().ToSingle<EventAggregator>();
    }
  }
}
