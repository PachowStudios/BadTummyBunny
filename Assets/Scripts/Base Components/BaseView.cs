using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseView : MonoBehaviour, IView
  {
    private Transform transformComponent;
    private Collider2D colliderComponent;
    private SpriteRenderer spriteRendererComponent;
    private Animator animatorComponent;
    private CharacterController2D characterControllerComponent;

    public Transform Transform => this.GetComponentIfNull(ref this.transformComponent);
    public Collider2D Collider => this.GetComponentIfNull(ref this.colliderComponent);
    public SpriteRenderer SpriteRenderer => this.GetComponentIfNull(ref this.spriteRendererComponent);
    public Animator Animator => this.GetComponentIfNull(ref this.animatorComponent);
    public CharacterController2D CharacterController => this.GetComponentIfNull(ref this.characterControllerComponent);

    public virtual void Dispose()
      => this.Destroy();

    public void SetRenderersEnabled(bool enableRenderers)
      => SpriteRenderer.enabled = enableRenderers;

    public void AlternateRenderersEnabled()
      => SpriteRenderer.enabled = !SpriteRenderer.enabled;
  }

  public abstract class BaseView<TModel> : BaseView, IView<TModel>
  {
    public abstract TModel Model { get; protected set; }
  }
}