using UnityEngine;

[AddComponentMenu("Environment/Respawn Point")]
public class RespawnPoint : MonoBehaviour
{
  [SerializeField] protected Vector3 localRespawnPoint = default(Vector3);

  private Animator animator;

  public bool Activated { get; private set; }
  public Vector3 Location => transform.TransformPoint(this.localRespawnPoint);

  protected Animator Animator => this.GetComponentIfNull(ref this.animator);

  public virtual void Activate()
  {
    if (Activated)
      return;

    Activated = true;
    Animator.SetBool("Activated", Activated);
    PlayActivateSound();
  }

  public virtual void Deactivate()
  {
    if (!Activated)
      return;

    Activated = false;
    this.animator.SetBool("Activated", Activated);
  }

  protected virtual void PlayActivateSound() 
    => SoundManager.PlaySFX(SoundManager.LoadFromGroup(SfxGroups.RespawnPoints));
}
