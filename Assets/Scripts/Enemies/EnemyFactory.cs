using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class EnemyFacadeFactory : FacadeFactory<Enemy.Settings, EnemyView, Enemy> { }

  public class EnemyFactory : IFactory<EnemyType, Enemy>, IFactory<EnemyView, Enemy>
  {
    [InstallerSettings]
    public class Settings : ScriptableObject
    {
      public List<Enemy.Settings> Enemies;

      public Lazy<Dictionary<EnemyType, Enemy.Settings>> MappedEnemies { get; }

      public Settings()
      {
        MappedEnemies = Lazy.From(() => this.Enemies.ToDictionary(s => s.Prefab.Type));
      }
    }

    [Inject] private Settings Config { get; set; }
    [Inject] private IInstantiator Instantiator { get; set; }
    [Inject] private EnemyFacadeFactory EnemyFacadeFactory { get; set; }

    public Enemy Create(EnemyType type)
      => Instantiator.InstantiatePrefab(
        GetSettingsFor(type).Prefab).Model;

    public Enemy Create(EnemyView view)
      => EnemyFacadeFactory.Create(GetSettingsFor(view.Type), view);

    private Enemy.Settings GetSettingsFor(EnemyType type)
      => Config.MappedEnemies.Value[type];
  }
}