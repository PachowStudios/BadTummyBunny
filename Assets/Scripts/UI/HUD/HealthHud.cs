using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/UI/HUD/Health")]
  public class HealthHud : MonoBehaviour,
    IHandles<PlayerHealthChangedMessage>,
    IHandles<PlayerHealthContainersChangedMessage>
  {
    [SerializeField] private Image heartPrefab = null;
    [SerializeField] private float spaceBetweenHearts = 0.125f;
    [Tooltip("Order is empty to full")]
    [SerializeField] private Sprite[] heartImages = new Sprite[5];

    private List<Image> healthContainers;

    [Inject(Tags.Player)]
    private IHasHealthContainers PlayerHealthContainers { get; set; }

    [Inject]
    private IEventAggregator EventAggregator { get; set; }

    private int HealthPerContainer => PlayerHealthContainers.HealthPerContainer;

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    private void Awake()
      => this.healthContainers = transform
        .GetComponentsInChildren<Image>()
        .OrderBy(h => h.rectTransform.anchoredPosition.x)
        .ToList();

    public void Handle(PlayerHealthContainersChangedMessage message)
    {
      var newHealthContainers = message.HealthContainers;

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

    public void Handle(PlayerHealthChangedMessage message)
    {
      var fullContainers = message.Health / HealthPerContainer;
      var partialHealth = message.Health % HealthPerContainer;

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
}
