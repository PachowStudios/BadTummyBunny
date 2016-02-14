using System.Collections.Generic;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class EnemyFactory : IFactory<EnemyType, Enemy>
  {
    [Inject] private IInstantiator Instantiator { get; set; }
    [Inject] private Dictionary<EnemyType, EnemySettings> MappedSettings { get; set; }

    public Enemy Create(EnemyType type)
      => Instantiator.InstantiatePrefab(MappedSettings[type].Prefab).Model;
  }
}
