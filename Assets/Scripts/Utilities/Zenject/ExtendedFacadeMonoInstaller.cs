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
        subContainer = context.Container.CreateSubContainer();
        subContainer.Install<StandardInstaller<TFacade>>();
        InstallSubContainerBindings(subContainer, instance);
        SubContainers.Add(instance, subContainer);
      }

      return subContainer.Resolve<TContract>(context);
    }
  }
}