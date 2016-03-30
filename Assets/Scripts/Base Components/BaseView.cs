using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseView : MonoBehaviour, IView
  {
    private Transform transformComponent;
    private Collider2D colliderComponent;
    private Animator animatorComponent;
    private CharacterController2D characterControllerComponent;
    private SpriteRenderer spriteRendererComponent;

    public Transform Transform => this.GetComponentIfNull(ref this.transformComponent);
    public Collider2D Collider => this.GetComponentIfNull(ref this.colliderComponent);
    public Animator Animator => this.GetComponentIfNull(ref this.animatorComponent);
    public CharacterController2D CharacterController => this.GetComponentIfNull(ref this.characterControllerComponent);
    public SpriteRenderer SpriteRenderer => this.GetComponentInChildrenIfNull(ref this.spriteRendererComponent);

    public virtual Vector3 Position => Transform.position;
    public virtual Vector3 CenterPoint => Collider.bounds.center;

    public virtual void Dispose()
      => this.Destroy();

    public virtual void Flip()
      => Transform.Flip();

    public void SetRenderersEnabled(bool enableRenderers)
      => SpriteRenderer.enabled = enableRenderers;

    public void AlternateRenderersEnabled()
      => SpriteRenderer.enabled = !SpriteRenderer.enabled;
  }

  public abstract class BaseView<TModel> : BaseView, IView<TModel>
    where TModel : class
  {
    public abstract TModel Model { get; protected set; }
  }
}