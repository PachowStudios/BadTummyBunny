using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Environment/Respawn Point")]
  public class RespawnPoint : BaseView
  {
    [SerializeField] private Vector3 localRespawnPoint = default(Vector3);

    public bool IsActivated { get; private set; }
    public Vector3 Location => Transform.TransformPoint(this.localRespawnPoint);

    public void Activate()
    {
      if (IsActivated)
        return;

      IsActivated = true;
      UpdateAnimation();
      PlayActivateSound();
    }

    public void Deactivate()
    {
      if (!IsActivated)
        return;

      IsActivated = false;
      UpdateAnimation();
    }

    private void UpdateAnimation()
      => Animator.SetBool("IsActivated", IsActivated);

    private static void PlayActivateSound() 
      => SoundManager.PlaySFXFromGroup(SfxGroup.RespawnPoints);
  }
}
