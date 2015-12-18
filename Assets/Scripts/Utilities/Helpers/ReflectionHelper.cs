using System;
using System.Reflection;

namespace PachowStudios
{
  public static class ReflectionHelper
  {
    public static T Create<T>(params object[] args)
      => (T)Activator.CreateInstance(
        typeof(T),
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
        null, args, null);
  }
}