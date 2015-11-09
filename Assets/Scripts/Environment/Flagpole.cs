using UnityEngine;

public class Flagpole : MonoBehaviour
{
  private Animator animator;

  public bool Activated { get; private set; }

  private Animator Animator => this.GetComponentIfNull(ref this.animator);

  public void Activate()
  {
    if (Activated)
      return;

    Activated = true;
    Animator.SetTrigger("Activate");
    PlayActivateSound();
  }

  private void PlayActivateSound() 
    => SoundManager.PlaySFXFromGroup(SfxGroups.RespawnPoints);
}
