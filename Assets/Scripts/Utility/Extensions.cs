using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Extensions
{
  #region Float
  public static int Sign(this float parent) => (int)Mathf.Sign(parent);

  public static float Abs(this float parent) => Mathf.Abs(parent);

  public static int RoundToInt(this float parent) => Mathf.RoundToInt(parent);

  public static float RoundToTenth(this float parent) => Mathf.RoundToInt(parent * 10f) / 10f;

  public static float RoundToHalf(this float parent) => Mathf.RoundToInt(parent * 2f) / 2f;

  public static float Clamp01(this float parent) => Mathf.Clamp01(parent);
  #endregion

  #region Vector2
  public static Vector3 ToVector3(this Vector2 parent) => parent.ToVector3(0f);

  public static Vector3 ToVector3(this Vector2 parent, float z) => new Vector3(parent.x, parent.y, z);

  [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
  public static bool IsZero(this Vector2 parent) => parent.x == 0f && parent.y == 0f;

  public static Vector2 Dot(this Vector2 parent, Vector2 other) => parent.Dot(other.x, other.y);

  public static Vector2 Dot(this Vector2 parent, float x, float y) => new Vector2(parent.x * x, parent.y * y);

  public static float RandomRange(this Vector2 parent) => Random.Range(parent.x, parent.y);
  #endregion

  #region Vector3
  public static Quaternion LookAt2D(this Vector3 parent, Vector3 target)
  {
    var targetPosition = target - parent;
    var angle = Mathf.Atan2(targetPosition.y, targetPosition.x) * Mathf.Rad2Deg;

    return Quaternion.Euler(new Vector3(0f, 0f, Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles.z));
  }

  public static Vector3 DirectionToRotation2D(this Vector3 parent)
  {
    var angle = Mathf.Atan2(parent.y, parent.x) * Mathf.Rad2Deg;

    return Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles;
  }

  public static float DistanceFrom(this Vector3 parent, Vector3 target) => Mathf.Sqrt(Mathf.Pow(parent.x - target.x, 2) + Mathf.Pow(parent.y - target.y, 2));
  #endregion

  #region Transform
  public static void Flip(this Transform parent)
  {
    parent.localScale = new Vector3(-parent.localScale.x, parent.localScale.y, parent.localScale.z);
  }

  public static Vector3 TransformPointLocal(this Transform parent, Vector3 target) => parent.TransformPoint(target) - parent.position;

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
  #endregion

  #region ParticleSystem
  public static void DetachAndDestroy(this ParticleSystem parent)
  {
    parent.transform.parent = null;
    parent.enableEmission = false;
    parent.gameObject.Destroy(parent.startLifetime);
  }
  #endregion

  #region Component
  public static T GetComponentIfNull<T>(this Component parent, ref T target) where T : Component
    => target ?? (target = parent.GetComponent<T>());

  public static T GetInterfaceIfNull<T>(this Component parent, ref T target) where T : class
    => target ?? (target = parent.GetInterface<T>());

  public static T GetInterface<T>(this Component parent) where T : class
    => parent.GetComponent(typeof(T)) as T;

  public static T[] GetInterfaces<T>(this Component parent) where T : class
    => Array.ConvertAll(parent.GetComponents(typeof(T)), c => c as T);
  #endregion

  #region MonoBehaviour
  public static void DestroyGameObject(this MonoBehaviour parent) => parent.gameObject.Destroy();
  #endregion

  #region GameObject
  public static void Destroy(this GameObject parent) => Object.Destroy(parent);

  public static void Destroy(this GameObject parent, float delay) => Object.Destroy(parent, delay);

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
  #endregion

  #region LayerMask
  public static bool ContainsLayer(this LayerMask parent, int layer) => ((parent.value & (1 << layer)) > 0);

  public static bool ContainsLayer(this LayerMask parent, GameObject obj) => parent.ContainsLayer(obj.layer);

  public static bool ContainsLayer(this LayerMask parent, Collider2D collider) => parent.ContainsLayer(collider.gameObject.layer);
  #endregion

  #region Generic Collection
  public static T GetRandom<T>(this IList<T> parent)
  {
    if (parent == null || parent.Count == 0)
      return default(T);

    return parent[Random.Range(0, parent.Count)];
  }

  public static T Pop<T>(this IList<T> parent)
  {
    if (parent == null || parent.Count == 0)
      return default(T);

    var lastIndex = parent.Count - 1;
    var lastItem = parent[lastIndex];

    parent.RemoveAt(lastIndex);

    return lastItem;
  }

  public static void ForEach<T>(this IEnumerable<T> parent, Action<T> action)
  {
    foreach (var item in parent)
      action(item);
  }

  public static bool IsEmpty<T>(this IEnumerable<T> parent) 
    => !parent.Any();
  #endregion

  #region Enum
  public static T GetAttributeOfType<T>(this Enum parent) where T : Attribute
  {
    var type = parent.GetType();
    var memInfo = type.GetMember(parent.ToString());
    var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);

    return (attributes.Length > 0) ? (T)attributes[0] : null;
  }

  public static string GetDescription(this Enum parent)
  {
    var attribute = parent.GetAttributeOfType<DescriptionAttribute>();

    return attribute != null ? attribute.Description : string.Empty;
  }
  #endregion

  #region Utility
  public static int ClampWrap(int value, int max) => ClampWrap(value, 0, max);

  public static int ClampWrap(int value, int min, int max)
  {
    Assert.IsGreaterThan(min, max);

    var range = max - min;

    while (value > max)
      value -= range;

    while (value < min)
      value += range;

    return value;
  }

  public static int RandomSign() => Random.value < 0.5 ? -1 : 1;

  public static float ConvertRange(float num, float oldMin, float oldMax, float newMin, float newMax)
  {
    num = Mathf.Clamp(num, oldMin, oldMax);

    var oldRange = oldMax - oldMin;
    var newRange = newMax - newMin;

    return (((num - oldMin) * newRange) / oldRange) + newMin;
  }

  public static float GetDecimal(float num)
  {
    var resultString = "0";
    float result;

    if (num.ToString().Split('.').Length == 2)
      resultString = "0." + num.ToString().Split('.')[1];

    float.TryParse(resultString, out result);

    return result;
  }

  public static Vector3 SuperSmoothLerp(Vector3 followOld, Vector3 targetOld, Vector3 targetNew, float elapsedTime, float lerpAmount)
  {
    var f = followOld - targetOld + (targetNew - targetOld) / (lerpAmount * elapsedTime);

    return targetNew - (targetNew - targetOld) / (lerpAmount * elapsedTime) + f * Mathf.Exp(-lerpAmount * elapsedTime);
  }

  public static Vector3 Vector3Range(Vector3 min, Vector3 max) => new Vector3(Random.Range(min.x, max.x),
                                                                              Random.Range(min.y, max.y),
                                                                              Random.Range(min.z, max.z));

  public static float UnitsToPixels(float units)
  {
    var worldPoint = Camera.main.ViewportToWorldPoint(Vector3.zero) + new Vector3(units, 0f);

    return Camera.main.WorldToScreenPoint(worldPoint).x;
  }
  #endregion
}
