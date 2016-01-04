using UnityEngine;

public class Flagpole : MonoBehaviour
{
  private Animator animatorComponent;

  public bool Activated { get; private set; }

  private Animator Animator => this.GetComponentIfNull(ref this.animatorComponent);

  public void Activate()
  {
    if (Activated)
      return;

    Activated = true;
    PlayActivateAnimation();
    PlayActivateSound();
  }

  private void PlayActivateAnimation()
    => Animator.SetTrigger("Activate");

  private static void PlayActivateSound() 
    => SoundManager.PlaySFXFromGroup(SfxGroup.RespawnPoints);
}
