using UnityEngine;

[AddComponentMenu("Environment/Respawn Point")]
public class RespawnPoint : MonoBehaviour
{
  [SerializeField] private Vector3 localRespawnPoint = default(Vector3);

  private Animator animator;

  public bool IsActivated { get; private set; }
  public Vector3 Location => transform.TransformPoint(this.localRespawnPoint);

  private Animator Animator => this.GetComponentIfNull(ref this.animator);

  public void Activate()
  {
    if (IsActivated)
      return;

    IsActivated = true;
    Animator.SetBool("IsActivated", IsActivated);
    PlayActivateSound();
  }

  public void Deactivate()
  {
    if (!IsActivated)
      return;

    IsActivated = false;
    this.animator.SetBool("IsActivated", IsActivated);
  }

  private void PlayActivateSound() 
    => SoundManager.PlaySFXFromGroup(SfxGroups.RespawnPoints);
}
