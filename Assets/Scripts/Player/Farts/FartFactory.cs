using System;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class FartFactory : IFactory<FartType, IFart>
  {
    [Inject] private DiContainer Container { get; set; }

    public IFart Create(FartType type)
    {
      var subContainer = Container.CreateSubContainer();

      subContainer.Bind<IEventAggregator>().ToSingle<EventAggregator>();

      return (IFart)subContainer.Instantiate(type.GetTypeMapping());
    }
  }
}