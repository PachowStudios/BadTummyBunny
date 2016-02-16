using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Zenject
{
  public static class ZenjectExtensions
  {
    public static void BindPriority<T>([NotNull] this DiContainer container, int priority)
      => ExecutionOrderInstaller.BindPriority<T>(container, priority);

    public static BindingConditionSetter BindSingle<TConcrete>([NotNull] this DiContainer container)
      => container.BindSingle(typeof(TConcrete));

    public static BindingConditionSetter BindSingle([NotNull] this DiContainer container, Type type)
      => container.Bind(type).ToSingle();

    public static void BindSingleWithInterfaces<TConcrete>([NotNull] this DiContainer container)
      => container.BindSingleWithInterfaces(typeof(TConcrete));

    public static void BindSingleWithInterfaces([NotNull] this DiContainer container, Type type)
    {
      container.Bind(type).ToSingle();
      container.BindAllInterfacesToSingle(type);
    }

    public static void BindInstanceWithInterfaces<TConcrete>([NotNull] this DiContainer container, TConcrete @object)
      => container.BindInstanceWithInterfaces(typeof(TConcrete), @object);

    public static void BindInstanceWithInterfaces([NotNull] this DiContainer container, Type type, object @object)
    {
      container.Bind(type).ToInstance(@object);
      container.BindAllInterfacesToInstance(type, @object);
    }

    public static BindingConditionSetter BindBaseInstance([NotNull] this DiContainer container, object @object)
      => container.Bind(@object.GetType()).ToInstance(@object);

    public static PublicFacadeBinder<TFacade> BindPublicFacade<TFacade>([NotNull] this DiContainer container, Action<DiContainer> installer)
      where TFacade : IFacade
      => new PublicFacadeBinder<TFacade>(container, installer);

    public static BindingConditionSetter ToSubContainer<TContract>([NotNull] this GenericBinder<TContract> binder, [NotNull] DiContainer subContainer)
      => binder.ToMethod(subContainer.Resolve<TContract>);

    public static BindingConditionSetter ToSubContainer<TContract, TConcrete>([NotNull] this GenericBinder<TContract> binder, [NotNull] DiContainer subContainer)
      where TConcrete : TContract
      => binder.ToMethod(c => subContainer.Resolve<TConcrete>());

    public static BindingConditionSetter ToSinglePrefab([NotNull] this TypeBinder binder, [NotNull] MonoBehaviour prefab)
      => binder.ToSinglePrefab(prefab.gameObject);

    public static BindingConditionSetter ToTransientPrefab([NotNull] this TypeBinder binder, [NotNull] MonoBehaviour prefab)
      => binder.ToTransientPrefab(prefab.gameObject);

    [NotNull]
    public static T InstantiatePrefab<T>([NotNull] this IInstantiator instantiator, [NotNull] T prefab)
      where T : MonoBehaviour
      => instantiator.InstantiatePrefabForComponent<T>(prefab.gameObject);
  }
}