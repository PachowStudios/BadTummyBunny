using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class EnemyFacadeFactory : FacadeFactory<EnemySettings, EnemyView, Enemy> { }

  public class EnemyFactory : IFactory<EnemyType, Enemy>, IFactory<EnemyView, Enemy>
  {
    [Inject] private EnemyFacadeFactory EnemyFacadeFactory { get; set; }
    [Inject] private IInstantiator Instantiator { get; set; }

    private Dictionary<EnemyType, EnemySettings> MappedSettings { get; }

    public EnemyFactory(List<EnemySettings> enemySettings)
    {
      MappedSettings = enemySettings.ToDictionary(s => s.Prefab.Type);
    }

    public Enemy Create(EnemyType type)
      => Instantiator.InstantiatePrefab(MappedSettings[type].Prefab).Model;

    public Enemy Create(EnemyView view)
      => EnemyFacadeFactory.Create(MappedSettings[view.Type], view);
  }
}
