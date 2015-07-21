using UnityEngine;
using System.Collections;

[AddComponentMenu("Utility/Particle System Sorting")]
[ExecuteInEditMode]
public sealed class ParticleSystemSorting : MonoBehaviour
{
	#region Fields
	public ParticleSystem partSystem;
	public string sortingLayer;
	public int sortingOrder;
	#endregion

	#region MonoBehaviour
	private void OnEnable()
	{
		partSystem.renderer.sortingLayerName = sortingLayer;
		partSystem.renderer.sortingOrder = sortingOrder;
	}
	#endregion
}
