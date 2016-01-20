using System;
using System.Collections.Generic;

namespace Zenject
{
  public class PublicFacadeBinder<TFacade>
    where TFacade : IFacade
  {
    private Action<DiContainer> Installer { get; }
    private DiContainer Container { get; }

    private DiContainer FacadeContainer { get; set; }

    public PublicFacadeBinder(DiContainer container, Action<DiContainer> installer)
    {
      Container = container;
      Installer = installer;
    }

    public DiContainer ToSingle()
    {
      AddValidator();

      Container.Bind<IInitializable>().ToLookup<TFacade>();
      Container.Bind<IDisposable>().ToLookup<TFacade>();
      Container.Bind<ITickable>().ToLookup<TFacade>();
      Container.Bind<ILateTickable>().ToLookup<TFacade>();
      Container.Bind<IFixedTickable>().ToLookup<TFacade>();
      Container.Bind<TFacade>().ToSingleMethod(c => FacadeContainer.Resolve<TFacade>());

      return FacadeContainer = FacadeFactory<TFacade>.CreateSubContainer(Container, Installer);
    }

    private void AddValidator()
      => Container.Bind<IValidatable>().ToInstance(
        new Validator(Container, Installer));

    private class Validator : IValidatable
    {
      private DiContainer Container { get; }
      private Action<DiContainer> InstallerFunc { get; }

      public Validator(DiContainer container, Action<DiContainer> installerFunc)
      {
        Container = container;
        InstallerFunc = installerFunc;
      }

      public IEnumerable<ZenjectResolveException> Validate()
        => FacadeFactory<TFacade>.Validate(Container, InstallerFunc);
    }
  }
}