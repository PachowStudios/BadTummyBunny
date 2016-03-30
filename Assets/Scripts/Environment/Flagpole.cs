using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Environment/Flagpole")]
  public class Flagpole : BaseView
  {
    public bool IsActivated { get; private set; }

    [Inject] private ILevelCompletionHandler LevelCompletionHandler { get; set; }

    public void Activate()
    {
      if (IsActivated)
        return;

      IsActivated = true;
      PlayActivateAnimation();
      PlayActivateSound();
      LevelCompletionHandler.CompleteLevel();
    }

    private void PlayActivateAnimation()
      => Animator.SetTrigger("Activate");

    private static void PlayActivateSound()
      => SoundManager.PlaySFXFromGroup(SfxGroup.RespawnPoints);
  }
}
