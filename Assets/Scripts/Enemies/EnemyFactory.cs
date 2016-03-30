using PachowStudios.Collections;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class EnemyFactory : IFactory<EnemyType, Enemy>
  {
    [Inject] private IInstantiator Instantiator { get; set; }
    [Inject] private IReadOnlyDictionary<EnemyType, EnemySettings> EnemySettings { get; set; }

    public Enemy Create(EnemyType type)
      => Instantiator.InstantiatePrefab(EnemySettings[type].Prefab).Model;
  }
}
