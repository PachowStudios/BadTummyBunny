using UnityEngine;

namespace PachowStudios
{
  [AddComponentMenu("Pachow Studios/Utilities/Particle System Sorting")]
  [ExecuteInEditMode]
  public sealed class ParticleSystemSorting : MonoBehaviour
  {
    [SerializeField] private string sortingLayer = "";
    [SerializeField] private int sortingOrder = 0;

    private void OnEnable()
    {
      var particleRenderer = GetComponent<ParticleSystem>().GetComponent<Renderer>();

      particleRenderer.sortingLayerName = this.sortingLayer;
      particleRenderer.sortingOrder = this.sortingOrder;
    }
  }
}
