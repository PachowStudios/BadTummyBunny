using System;
using PachowStudios.Collections;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class FartFactory : IFactory<FartType, IFart>
  {
    [Inject] private DiContainer Container { get; set; }
    [Inject] private IReadOnlyDictionary<FartType, FartSettings> FartSettings { get; set; }

    public IFart Create(FartType type)
    {
      var subContainer = Container.CreateSubContainer();
      var settings = FartSettings[type];

      subContainer.BindBaseInstance(settings);
      subContainer.Bind<FartView>().ToTransientPrefab(settings.Prefab);

      return (IFart)subContainer.Instantiate(type.GetTypeMapping());
    }
  }
}