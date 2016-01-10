using System;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class FartFactory : IFactory<FartType, IFart>
  {
    [Inject] private IInstantiator Instantiator { get; set; }

    public IFart Create(FartType type)
      => (IFart)Instantiator.Instantiate(type.GetTypeMapping());
  }
}