using System;
using JetBrains.Annotations;
using PachowStudios;

namespace UnityEngine
{
  public static class UnityExtensions
  {
    // ReSharper disable ConvertConditionalTernaryToNullCoalescing
    // The ?? operator doesn't use Unity's overloaded null check
    [CanBeNull]
    public static T GetComponentIfNull<T>([NotNull] this Component component, [CanBeNull] ref T target)
      where T : class
      => component.gameObject.GetComponentIfNull(ref target);

    [CanBeNull]
    public static T GetComponentIfNull<T>([NotNull] this GameObject gameObject, [CanBeNull] ref T target)
      where T : class
      => target == null ? (target = gameObject.GetComponent<T>()) : target;

    [CanBeNull]
    public static T GetComponentInParentIfNull<T>([NotNull] this Component component, [CanBeNull] ref T target)
      where T : class
      => component.gameObject.GetComponentInParentIfNull(ref target);

    [CanBeNull]
    public static T GetComponentInParentIfNull<T>([NotNull] this GameObject gameObject, [CanBeNull] ref T target)
      where T : class
      => target == null ? (target = gameObject.GetComponentInParent<T>()) : target;

    [CanBeNull]
    public static T GetComponentInChildrenIfNull<T>([NotNull] this Component component, [CanBeNull] ref T target)
      where T : class
      => component.gameObject.GetComponentInChildrenIfNull(ref target);

    [CanBeNull]
    public static T GetComponentInChildrenIfNull<T>([NotNull] this GameObject gameObject, [CanBeNull] ref T target)
      where T : class
      => target == null ? (target = gameObject.GetComponentInChildren<T>()) : target;
    // ReSharper restore ConvertConditionalTernaryToNullCoalescing

    [NotNull]
    public static TModel GetViewModel<TModel>([NotNull] this Component component)
      where TModel : class
      => component.gameObject.GetViewModel<TModel>();

    [NotNull]
    public static TModel GetViewModel<TModel>([NotNull] this GameObject gameObject)
      where TModel : class
    {
      var view = gameObject.GetComponent<IView<TModel>>();

      if (view == null)
        throw new InvalidOperationException($"GetViewModel failed to get model of type {typeof(TModel)}");

      return view.Model;
    }

    public static void Destroy([NotNull] this MonoBehaviour monoBehaviour)
      => monoBehaviour.gameObject.Destroy();

    public static void Destroy([NotNull] this MonoBehaviour monoBehaviour, float delay)
      => monoBehaviour.gameObject.Destroy(delay);

    public static void Destroy([NotNull] this GameObject gameObject)
      => Object.Destroy(gameObject);

    public static void Destroy([NotNull] this GameObject gameObject, float delay)
      => Object.Destroy(gameObject, delay);

    [NotNull]
    public static GameObject HideInHierarchy([NotNull] this GameObject gameObject)
    {
      gameObject.hideFlags |= HideFlags.HideInHierarchy;

      gameObject.SetActive(false);
      gameObject.SetActive(true);

      return gameObject;
    }

    [NotNull]
    public static GameObject UnhideInHierarchy([NotNull] this GameObject gameObject)
    {
      gameObject.hideFlags &= ~HideFlags.HideInHierarchy;

      gameObject.SetActive(false);
      gameObject.SetActive(true);

      return gameObject;
    }

    public static Vector3 TransformPoint([NotNull] this Transform transform, float x, float y)
      => transform.TransformPoint(x, y, 0f);

    /// <summary>
    /// Sets the position, rotation, and localScale to that of the target transform.
    /// </summary>
    public static void AlignWith([NotNull] this Transform transform, Transform target)
    {
      transform.position = target.position;
      transform.rotation = target.rotation;
      transform.localScale = target.localScale;
    }

    public static void Flash([NotNull] this SpriteRenderer spriteRenderer, Color color, float time)
    {
      spriteRenderer.color = color;
      Wait.ForSeconds(time, spriteRenderer.ResetColor);
    }

    public static void ResetColor([NotNull] this SpriteRenderer spriteRenderer)
      => spriteRenderer.color = Color.white;

    public static void DetachAndDestroy([NotNull] this ParticleSystem particleSystem)
    {
      particleSystem.transform.parent = null;
      particleSystem.SetEmissionEnabled(false);
      particleSystem.gameObject.Destroy(particleSystem.startLifetime);
    }

    public static void SetEmissionEnabled([NotNull] this ParticleSystem particleSystem, bool enabled)
    {
      var emission = particleSystem.emission;
      emission.enabled = enabled;
    }

    public static bool IsVisible([NotNull] this CanvasGroup canvasGroup)
      => !canvasGroup.alpha.IsZero();

    public static void SetVisibility([NotNull] this CanvasGroup canvasGroup, bool visible)
    {
      canvasGroup.alpha = visible.ToInt();
      canvasGroup.interactable = visible;
      canvasGroup.blocksRaycasts = visible;
    }

    public static bool HasLayer(this LayerMask mask, int layer)
      => (mask.value & (1 << layer)) > 0;

    public static bool HasLayer(this LayerMask mask, [NotNull] GameObject obj)
      => mask.HasLayer(obj.layer);

    public static bool HasLayer(this LayerMask mask, [NotNull] Collider2D collider)
      => mask.HasLayer(collider.gameObject.layer);

    public static float UnitsToPixels([NotNull] this Camera camera, float units)
      => camera.WorldToScreenPoint(camera.ViewportToWorldPoint(Vector3.zero).AddX(units)).x;
  }
}