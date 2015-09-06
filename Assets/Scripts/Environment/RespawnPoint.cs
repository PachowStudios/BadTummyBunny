using UnityEngine;

[AddComponentMenu("Environment/Respawn Point")]
public sealed class RespawnPoint : MonoBehaviour
{
	#region Fields
	[SerializeField]
	private Vector3 localRespawnPoint = default(Vector3);

	private bool activated = false;

	private Animator animator;
	#endregion

	#region Public Properties
	public Vector3 Location
	{ get { return transform.TransformPoint(localRespawnPoint); } }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		animator = GetComponent<Animator>();
	}
	#endregion

	#region Public Methods
	public void Activate()
	{
		if (activated) return;

		activated = true;
		animator.SetBool("Activated", activated);
		PlayActivateSound();
	}

	public void Deactivate()
	{
		if (!activated) return;

		activated = false;
		animator.SetBool("Activated", activated);
	}
	#endregion

	#region Internal Helper Methods
	private void PlayActivateSound()
	{
		SoundManager.PlaySFX(SoundManager.LoadFromGroup(SfxGroups.RespawnPoints));
	}
	#endregion
}
