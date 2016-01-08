using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class EnemyFacadeFactory : FacadeFactory<Enemy.Settings, EnemyView, Enemy> { }

  public class EnemyFactory : IFactory<EnemyType, Enemy>, IFactory<EnemyView, Enemy>
  {
    [Inject] private EnemyFacadeFactory EnemyFacadeFactory { get; set; }
    [Inject] private IInstantiator Instantiator { get; set; }

    private Dictionary<EnemyType, Enemy.Settings> MappedEnemies { get; }

    public EnemyFactory(IEnumerable<Enemy.Settings> enemySettings)
    {
      MappedEnemies = enemySettings.ToDictionary(s => s.Prefab.Type);
    }

    public Enemy Create(EnemyType type)
      => Instantiator.InstantiatePrefab(MappedEnemies[type].Prefab).Model;

    public Enemy Create(EnemyView view)
      => EnemyFacadeFactory.Create(MappedEnemies[view.Type], view);
  }
}
