using System;
using JetBrains.Annotations;
using PachowStudios;

namespace UnityEngine
{
  public static class UnityExtensions
  {
    public static T GetComponentInParentIfNull<T>(this Component component, ref T target)
      where T : Component
      => target ?? (target = component.GetComponentInParent<T>());

    public static T GetComponentIfNull<T>(this Component component, ref T target)
      where T : Component
      => target ?? (target = component.GetComponent<T>());

    public static T GetInterfaceIfNull<T>(this Component component, ref T target)
      where T : class
      => target ?? (target = component.GetInterface<T>());

    public static T GetInterface<T>(this Component component)
      where T : class
      => component.GetComponent(typeof(T)) as T;

    public static T GetInterface<T>(this GameObject gameObject)
      where T : class
      => gameObject.GetComponent(typeof(T)) as T;

    public static T[] GetInterfaces<T>(this Component component)
      where T : class
      => Array.ConvertAll(component.GetComponents(typeof(T)), c => c as T);

    public static TModel GetViewModel<TModel>(this Component component)
      => component.GetInterface<IView<TModel>>().Model;

    public static void Dispose(this MonoBehaviour monoBehaviour)
      => monoBehaviour.DestroyGameObject();

    public static void DestroyGameObject(this MonoBehaviour monoBehaviour)
      => monoBehaviour.gameObject.Destroy();

    public static void Destroy(this GameObject gameObject)
      => Object.Destroy(gameObject);

    public static void Destroy(this GameObject gameObject, float delay)
      => Object.Destroy(gameObject, delay);

    public static void DetachAndDestroy(this ParticleSystem parent)
    {
      parent.transform.parent = null;
      parent.enableEmission = false;
      parent.gameObject.Destroy(parent.startLifetime);
    }

    public static GameObject HideInHierarchy(this GameObject gameObject)
    {
      gameObject.hideFlags |= HideFlags.HideInHierarchy;

      gameObject.SetActive(false);
      gameObject.SetActive(true);

      return gameObject;
    }

    public static GameObject UnhideInHierarchy(this GameObject gameObject)
    {
      gameObject.hideFlags &= ~HideFlags.HideInHierarchy;

      gameObject.SetActive(false);
      gameObject.SetActive(true);

      return gameObject;
    }

    public static void Flash([NotNull] this SpriteRenderer spriteRenderer, Color color, float time)
    {
      spriteRenderer.color = color;
      Wait.ForSeconds(time, spriteRenderer.ResetColor);
    }

    public static void ResetColor([NotNull] this SpriteRenderer spriteRenderer)
      => spriteRenderer.color = Color.white;

    public static bool HasLayer(this LayerMask parent, int layer)
      => (parent.value & (1 << layer)) > 0;

    public static bool HasLayer(this LayerMask parent, GameObject obj)
      => parent.HasLayer(obj.layer);

    public static bool HasLayer(this LayerMask parent, Collider2D collider)
      => parent.HasLayer(collider.gameObject.layer);

    public static float UnitsToPixels(this Camera camera, float units)
      => camera.WorldToScreenPoint(camera.ViewportToWorldPoint(Vector3.zero).AddX(units)).x;
  }
}