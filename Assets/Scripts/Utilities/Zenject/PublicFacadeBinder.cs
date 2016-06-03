using System;
using System.Collections.Generic;

namespace Zenject
{
  public class PublicFacadeBinder<TFacade>
    where TFacade : IFacade
  {
    private DiContainer Container { get; }
    private Action<DiContainer> Installer { get; }

    private DiContainer FacadeContainer { get; set; }

    public PublicFacadeBinder(DiContainer container, Action<DiContainer> installer)
    {
      Container = container;
      Installer = installer;

      CreateSubContainer();
    }

    public PublicFacadeBinder<TFacade> InjectFromSubContainer<TContract, TTarget>()
    {
      Container
        .Bind<TContract>()
        .ToSubContainer(FacadeContainer)
        .WhenInjectedInto<TTarget>();

      return this;
    }

    private void CreateSubContainer()
    {
      AddValidator();
      Container.Bind<TFacade>().ToSingleMethod(c => FacadeContainer.Resolve<TFacade>());
      FacadeContainer = FacadeFactory<TFacade>.CreateSubContainer(Container, Installer);
    }

    private void AddValidator()
      => Container.Bind<IValidatable>().ToInstance(new Validator(this));

    private class Validator : IValidatable
    {
      private PublicFacadeBinder<TFacade> Binder { get; }

      public Validator(PublicFacadeBinder<TFacade> binder)
      {
        Binder = binder;
      }

      public IEnumerable<ZenjectResolveException> Validate()
        => FacadeFactory<TFacade>.Validate(Binder.Container, Binder.Installer);
    }
  }
}