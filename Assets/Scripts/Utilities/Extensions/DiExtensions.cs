using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Zenject
{
  public static class DiExtensions
  {
    public static void BindSingle<TConcrete>([NotNull] this DiContainer container)
      => container.BindSingle(typeof(TConcrete));

    public static void BindSingle([NotNull] this DiContainer container, Type type)
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

    public static BindingConditionSetter ToIFactory<T>([NotNull] this GenericBinder<T> binder)
      => binder.ToMethod(c => c.Container.Resolve<IFactory<T>>().Create());

    public static void ToIFactoryWithContext<TParam1, T>([NotNull] this GenericBinder<T> binder)
      => binder
        .ToMethod(c => c.Container
          .Resolve<IFactory<TParam1, T>>()
          .Create((TParam1)c.ObjectInstance))
        .WhenInjectedInto<TParam1>();

    public static BindingConditionSetter ToSinglePrefab([NotNull] this TypeBinder binder, [NotNull] MonoBehaviour prefab)
      => binder.ToSinglePrefab(prefab.gameObject);

    public static T InstantiatePrefab<T>([NotNull] this IInstantiator instantiator, [NotNull] T prefab)
      where T : MonoBehaviour
      => instantiator.InstantiatePrefabForComponent<T>(prefab.gameObject);
  }
}