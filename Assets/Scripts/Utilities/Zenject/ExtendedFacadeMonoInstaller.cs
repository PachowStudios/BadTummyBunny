using System;
using System.Collections.Generic;
using Zenject;

namespace PachowStudios
{
  public abstract class ExtendedFacadeMonoInstaller<TTarget, TFacade> : MonoInstaller
    where TTarget : class
    where TFacade : IFacade
  {
    private Dictionary<TTarget, DiContainer> SubContainers { get; } = new Dictionary<TTarget, DiContainer>();

    protected abstract void InstallSubContainerBindings(DiContainer subContainer, TTarget instance);

    protected void BindToSubContainer<TContract>()
      => Container
        .Bind<TContract>()
        .ToMethod(ResolveFromSubContainer<TContract>)
        .WhenInjectedInto<TTarget>();

    protected TContract ResolveFromSubContainer<TContract>(InjectContext context)
    {
      var instance = (TTarget)context.ObjectInstance;
      DiContainer subContainer;

      if (!SubContainers.TryGetValue(instance, out subContainer))
      {
        subContainer = CreateSubContainer(context);
        InstallSubContainerBindings(subContainer, instance);
        SubContainers.Add(instance, subContainer);
      }

      return subContainer.Resolve<TContract>(context);
    }

    private DiContainer CreateSubContainer(InjectContext context)
    {
      var subContainer = context.Container.CreateSubContainer();

      subContainer.Install<StandardInstaller<TFacade>>();

      Container.Bind<IInitializable>().ToSubContainer<IInitializable, TFacade>(subContainer);
      Container.Bind<IDisposable>().ToSubContainer<IDisposable, TFacade>(subContainer);
      Container.Bind<ITickable>().ToSubContainer<ITickable, TFacade>(subContainer);
      Container.Bind<ILateTickable>().ToSubContainer<ILateTickable, TFacade>(subContainer);
      Container.Bind<IFixedTickable>().ToSubContainer<IFixedTickable, TFacade>(subContainer);

      return subContainer;
    }
  }
}