using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Extensions
{
  public static int Sign(this float parent)
    => (int)Mathf.Sign(parent);

  public static float Abs(this float parent)
    => Mathf.Abs(parent);

  public static int RoundToInt(this float parent)
    => Mathf.RoundToInt(parent);

  public static float RoundToTenth(this float parent)
    => Mathf.RoundToInt(parent * 10f) / 10f;

  public static float RoundToHalf(this float parent)
    => Mathf.RoundToInt(parent * 2f) / 2f;

  public static float Clamp01(this float parent)
    => Mathf.Clamp01(parent);

  public static void Flip(this Transform parent)
    => parent.localScale = new Vector3(-parent.localScale.x, parent.localScale.y, parent.localScale.z);

  public static Vector3 TransformPointLocal(this Transform parent, Vector3 target)
    => parent.TransformPoint(target) - parent.position;

  public static void CorrectScaleForRotation(this Transform parent, Vector3 target, bool correctY = false)
  {
    var flipY = target.z > 90f && target.z < 270f;

    target.y = correctY && flipY ? 180f : 0f;

    var newScale = parent.localScale;

    newScale.x = 1f;
    newScale.y = flipY ? -1f : 1f;
    parent.localScale = newScale;
    parent.rotation = Quaternion.Euler(target);
  }

  public static void DetachAndDestroy(this ParticleSystem parent)
  {
    parent.transform.parent = null;
    parent.enableEmission = false;
    parent.gameObject.Destroy(parent.startLifetime);
  }

  public static T GetComponentInParentIfNull<T>(this Component parent, ref T target)
    where T : Component
    => target ?? (target = parent.GetComponentInParent<T>());

  public static T GetComponentIfNull<T>(this Component parent, ref T target)
    where T : Component
    => target ?? (target = parent.GetComponent<T>());

  public static T GetInterfaceIfNull<T>(this Component parent, ref T target)
    where T : class
    => target ?? (target = parent.GetInterface<T>());

  public static T GetInterface<T>(this Component parent)
    where T : class
    => parent.GetComponent(typeof(T)) as T;

  public static T[] GetInterfaces<T>(this Component parent)
    where T : class
    => Array.ConvertAll(parent.GetComponents(typeof(T)), c => c as T);

  public static T GetInterface<T>(this GameObject gameObject)
    where T : class
    => gameObject.GetComponent(typeof(T)) as T;

  public static void DestroyGameObject(this MonoBehaviour parent)
    => parent.gameObject.Destroy();

  public static void Destroy(this GameObject parent)
    => Object.Destroy(parent);

  public static void Destroy(this GameObject parent, float delay)
    => Object.Destroy(parent, delay);

  public static void HideInHierarchy(this GameObject parent)
  {
    parent.hideFlags |= HideFlags.HideInHierarchy;

    parent.SetActive(false);
    parent.SetActive(true);
  }

  public static void UnhideInHierarchy(this GameObject parent)
  {
    parent.hideFlags &= ~HideFlags.HideInHierarchy;

    parent.SetActive(false);
    parent.SetActive(true);
  }

  public static bool ContainsLayer(this LayerMask parent, int layer)
    => (parent.value & (1 << layer)) > 0;

  public static bool ContainsLayer(this LayerMask parent, GameObject obj)
    => parent.ContainsLayer(obj.layer);

  public static bool ContainsLayer(this LayerMask parent, Collider2D collider)
    => parent.ContainsLayer(collider.gameObject.layer);

  public static T GetRandom<T>(this IList<T> parent)
  {
    if (parent == null || parent.IsEmpty())
      return default(T);

    return parent[Random.Range(0, parent.Count)];
  }

  public static T Pop<T>(this IList<T> parent)
  {
    if (parent == null || parent.IsEmpty())
      return default(T);

    var lastIndex = parent.Count - 1;
    var lastItem = parent[lastIndex];

    parent.RemoveAt(lastIndex);

    return lastItem;
  }

  [NotNull]
  public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
  {
    if (!dictionary.ContainsKey(key) || dictionary[key] == null)
      dictionary[key] = factory.Invoke();

    return dictionary[key];
  }

  [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
  public static IEnumerable<T> ForEach<T>(this IEnumerable<T> parent, Action<T> action)
  {
    foreach (var item in parent)
      action?.Invoke(item);

    return parent;
  }

  [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
  public static IEnumerable<T> ForEach<T>(this IEnumerable<T> parent, Action action)
  {
    // ReSharper disable once UnusedVariable
    foreach (var item in parent)
      action?.Invoke();

    return parent;
  }

  public static bool None<T>(this IEnumerable<T> parent, Func<T, bool> predicate)
    => !parent.Any(predicate);

  public static bool IsEmpty<T>(this IEnumerable<T> parent)
    => !parent.Any();

  public static bool HasSingle<T>(this IEnumerable<T> collection)
    => collection.Count() == 1;

  public static bool HasMultiple<T>(this IEnumerable<T> collection)
    => collection.Count() > 1;

  public static bool IsAssignableFrom<T>(this Type parent)
    => parent.IsAssignableFrom(typeof(T));

  public static T GetAttributeOfType<T>(this PropertyInfo propertyInfo, bool inherit = false)
    => (T)propertyInfo.GetCustomAttributes(typeof(T), inherit).FirstOrDefault();

  public static T GetAttributeOfType<T>(this Enum parent)
    where T : Attribute
    => (T)parent
      .GetType()
      .GetMember(parent.ToString()).First()
      .GetCustomAttributes(typeof(T), false).FirstOrDefault();

  public static string GetDescription(this Enum parent)
    => parent.GetAttributeOfType<DescriptionAttribute>()?.Description
    ?? string.Empty;

  public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type targetType, bool inherit = false)
    where T : Attribute
    => targetType
      .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
      .Where(p => Attribute.IsDefined(p, typeof(T), inherit))
      .ToList();

  public static int RandomSign()
    => Random.value < 0.5 ? -1 : 1;

  public static float ConvertRange(float num, float oldMin, float oldMax, float newMin, float newMax)
    => (((((Mathf.Clamp(num, oldMin, oldMax) - oldMin) * newMax) - newMin) / oldMax) - oldMin) + newMin;

  public static float GetDecimal(float num)
  {
    var resultString = "0";
    float result;

    if (num.ToString().Split('.').Length == 2)
      resultString = "0." + num.ToString().Split('.')[1];

    float.TryParse(resultString, out result);

    return result;
  }

  public static float UnitsToPixels(this Camera camera, float units)
    => camera.WorldToScreenPoint(camera.ViewportToWorldPoint(Vector3.zero).AddX(units)).x;

  public static void BindLifetimeSingleton<T>(this DiContainer container)
    where T : IInitializable, IDisposable
  {
    container.Bind<IInitializable>().ToSingle<T>();
    container.Bind<IDisposable>().ToSingle<T>();
  }
}
