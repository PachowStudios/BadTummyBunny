using UnityEngine;

[AddComponentMenu("Utility/Particle System Sorting")]
[ExecuteInEditMode]
public sealed class ParticleSystemSorting : MonoBehaviour
{
  [SerializeField] private ParticleSystem partSystem = null;
  [SerializeField] private string sortingLayer = "";
  [SerializeField] private int sortingOrder = 0;

  private void OnEnable()
  {
    this.partSystem.renderer.sortingLayerName = this.sortingLayer;
    this.partSystem.renderer.sortingOrder = this.sortingOrder;
  }
}
