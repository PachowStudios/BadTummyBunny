using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseView<TModel> : MonoBehaviour, IView<TModel>
  {
    private Transform transformComponent;
    private Collider2D colliderComponent;
    private SpriteRenderer spriteRendererComponent;
    private Animator animatorComponent;
    private CharacterController2D characterControllerComponent;

    public abstract TModel Model { get; protected set; }

    public Transform Transform => this.GetComponentIfNull(ref this.transformComponent);
    public Collider2D Collider => this.GetComponentIfNull(ref this.colliderComponent);
    public SpriteRenderer SpriteRenderer => this.GetComponentIfNull(ref this.spriteRendererComponent);
    public Animator Animator => this.GetComponentIfNull(ref this.animatorComponent);
    public CharacterController2D CharacterController => this.GetComponentIfNull(ref this.characterControllerComponent);

    public virtual void Dispose()
      => this.DestroyGameObject();

    public void SetRenderersEnabled(bool enableRenderers)
      => SpriteRenderer.enabled = enableRenderers;

    public void AlternateRenderersEnabled()
      => SpriteRenderer.enabled = !SpriteRenderer.enabled;
  }
}