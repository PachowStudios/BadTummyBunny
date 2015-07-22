using UnityEngine;

public class Flagpole : MonoBehaviour
{
	#region Fields
	private bool activated = false;

	private Animator animator;
	#endregion

	#region Public Properties
	public bool Activated
	{ get { return activated; } }
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
		animator.SetTrigger("Activate");
		PlayActivateSound();
	}
	#endregion

	#region Internal Helper Methods
	private void PlayActivateSound()
	{
		SoundManager.PlaySFX(SoundManager.LoadFromGroup(SfxGroups.RespawnPoints));
	}
	#endregion
}
