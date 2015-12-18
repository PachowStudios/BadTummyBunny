using System;

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

    public static void DestroyGameObject(this MonoBehaviour parent)
      => parent.gameObject.Destroy();

    public static void Destroy(this GameObject parent)
      => Object.Destroy(parent);

    public static void Destroy(this GameObject parent, float delay)
      => Object.Destroy(parent, delay);

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