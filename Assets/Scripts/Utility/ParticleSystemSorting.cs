using UnityEngine;

[AddComponentMenu("Utility/Particle System Sorting")]
[ExecuteInEditMode]
public sealed class ParticleSystemSorting : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem partSystem = null;
	[SerializeField]
	private string sortingLayer = "";
	[SerializeField]
	private int sortingOrder = 0;

	private void OnEnable()
	{
		partSystem.renderer.sortingLayerName = sortingLayer;
		partSystem.renderer.sortingOrder = sortingOrder;
	}
}
