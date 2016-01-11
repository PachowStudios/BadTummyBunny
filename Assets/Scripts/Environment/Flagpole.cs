using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Environment/Flagpole")]
  public class Flagpole : BaseView
  {
    public bool Activated { get; private set; }

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
}
