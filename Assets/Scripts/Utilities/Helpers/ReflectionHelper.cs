using System;
using System.Reflection;
using JetBrains.Annotations;

namespace PachowStudios
{
  public static class ReflectionHelper
  {
    [NotNull, Pure]
    public static T Create<T>([CanBeNull] params object[] args)
      => (T)Activator.CreateInstance(
        typeof(T),
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        null, args, null);
  }
}