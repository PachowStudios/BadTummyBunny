using UnityEngine;

public class Flagpole : MonoBehaviour
{
  private Animator animator;

  public virtual bool Activated { get; protected set; }

  protected Animator Animator => this.GetComponentIfNull(ref this.animator);

  public virtual void Activate()
  {
    if (Activated)
      return;

    Activated = true;
    Animator.SetTrigger("Activate");
    PlayActivateSound();
  }

  protected virtual void PlayActivateSound() 
    => SoundManager.PlaySFX(SoundManager.LoadFromGroup(SfxGroups.RespawnPoints));
}
