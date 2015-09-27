using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/HUD/Health")]
public class HealthHud : MonoBehaviour
{
  [SerializeField] protected Image heartPrefab;
  [SerializeField] protected float spaceBetweenHearts = 0.125f;
  [SerializeField, Tooltip("Order is empty to full")] protected Sprite[] heartImages = new Sprite[5];

  protected List<Image> healthContainers;

  public static HealthHud Instance { get; private set; }

  protected int HealthPerContainer => Player.Instance.HealthContainers.HealthPerContainer;

  protected virtual void Awake()
  {
    Instance = this;

    this.healthContainers = transform.GetComponentsInChildren<Image>()
                                     .OrderBy(h => h.rectTransform.anchoredPosition.x)
                                     .ToList();
  }

  protected virtual void Start()
  {
    Player.Instance.HealthContainers.HealthContainersChanged += OnHealthContainersChanged;
    Player.Instance.Health.HealthChanged += OnHealthChanged;

    OnHealthContainersChanged(Player.Instance.HealthContainers.HealthContainers);
    OnHealthChanged(Player.Instance.Health.Health);
  }

  protected virtual void OnDestroy()
  {
    Player.Instance.HealthContainers.HealthContainersChanged -= OnHealthContainersChanged;
    Player.Instance.Health.HealthChanged -= OnHealthChanged;
  }

  protected virtual void OnHealthContainersChanged(int newHealthContainers)
  {
    if (this.healthContainers.Count < newHealthContainers)
    {
      while (this.healthContainers.Count < newHealthContainers)
      {
        var newHeart = (Image)Instantiate(this.heartPrefab, Vector3.zero, Quaternion.identity);

        newHeart.transform.SetParent(transform, false);
        newHeart.sprite = this.heartImages[0];

        if (this.healthContainers.Count > 0)
          newHeart.rectTransform.anchoredPosition = new Vector2(this.healthContainers.Last().rectTransform.offsetMax.x + this.spaceBetweenHearts, this.healthContainers.Last().rectTransform.anchoredPosition.y);

        this.healthContainers.Add(newHeart);
      }
    }
    else if (this.healthContainers.Count > newHealthContainers)
    {
      while (this.healthContainers.Count > newHealthContainers)
        Destroy(this.healthContainers.Pop().gameObject);
    }
  }

  protected virtual void OnHealthChanged(int newHealth)
  {
    var fullContainers = newHealth / HealthPerContainer;
    var partialHealth = newHealth % HealthPerContainer;

    foreach (var container in this.healthContainers)
    {
      if (fullContainers > 0)
        container.sprite = this.heartImages[HealthPerContainer];
      else if (fullContainers == 0)
        container.sprite = this.heartImages[partialHealth];
      else
        container.sprite = this.heartImages[0];

      fullContainers--;
    }
  }
}
