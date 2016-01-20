using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PachowStudios
{
  public abstract class ViewContainerMonoInstaller<TView> : MonoInstaller
    where TView : MonoBehaviour
  {
    private Dictionary<TView, DiContainer> ViewContainers { get; } = new Dictionary<TView, DiContainer>();

    private Lazy<DiContainer> GlobalViewContainer { get; }

    protected ViewContainerMonoInstaller()
    {
      GlobalViewContainer = Lazy.From(CreateGlobalViewContainer);
    }

    protected abstract void InstallViewContainerBindings(DiContainer viewContainer);

    protected void BindToViewContainer<TContract>()
      => Container
        .Bind<TContract>()
        .ToMethod(ResolveFromViewContainer<TContract>)
        .WhenInjectedInto<TView>();

    protected BindingConditionSetter BindToGlobalViewContainer<TContract>()
      => Container
        .Bind<TContract>()
        .ToMethod(ResolveFromGlobalViewContainer<TContract>);

    protected TContract ResolveFromViewContainer<TContract>(InjectContext context)
    {
      var view = (TView)context.ObjectInstance;
      DiContainer viewContainer;

      if (!ViewContainers.TryGetValue(view, out viewContainer))
      {
        viewContainer = context.Container.CreateSubContainer();
        ViewContainers.Add(view, viewContainer);
        InstallViewContainerBindings(viewContainer);
      }

      return viewContainer.Resolve<TContract>(context);
    }

    protected TContract ResolveFromGlobalViewContainer<TContract>(InjectContext context)
      => GlobalViewContainer.Value.Resolve<TContract>(context);

    private DiContainer CreateGlobalViewContainer()
    {
      var globalContainer = Container.CreateSubContainer();

      InstallViewContainerBindings(globalContainer);

      return globalContainer;
    }
  }
}