using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/UI/HUD/Health HUD")]
  public class HealthHud : BaseView,
    IHandles<PlayerHealthChangedMessage>,
    IHandles<PlayerHealthContainersChangedMessage>
  {
    [SerializeField] private Image heartPrefab = null;
    [SerializeField] private float spaceBetweenHearts = 0.125f;
    [Tooltip("Order is empty to full")]
    [SerializeField] private Sprite[] heartImages = new Sprite[5];

    [Inject] private Player Player { get; set; }
    [Inject] private IEventAggregator EventAggregator { get; set; }

    private List<Image> HealthContainers { get; set; }

    private int HealthPerContainer => Player.HealthContainers.HealthPerContainer;

    [PostInject]
    private void PostInject()
      => EventAggregator.Subscribe(this);

    private void Awake()
      => HealthContainers = Transform
        .GetComponentsInChildren<Image>()
        .OrderBy(h => h.rectTransform.anchoredPosition.x)
        .ToList();

    public void Handle(PlayerHealthContainersChangedMessage message)
    {
      var newHealthContainers = message.HealthContainers;

      while (HealthContainers.HasLessThan(newHealthContainers))
      {
        var newHeart = (Image)Instantiate(this.heartPrefab, Vector3.zero, Quaternion.identity);

        newHeart.transform.SetParent(Transform, false);
        newHeart.sprite = this.heartImages.First();

        if (HealthContainers.Any())
          newHeart.rectTransform.anchoredPosition = new Vector2(
            HealthContainers.Last().rectTransform.offsetMax.x + this.spaceBetweenHearts,
            HealthContainers.Last().rectTransform.anchoredPosition.y);

        HealthContainers.Add(newHeart);
      }

      while (HealthContainers.HasMoreThan(newHealthContainers))
        HealthContainers.Pop().Destroy();
    }

    public void Handle(PlayerHealthChangedMessage message)
    {
      var fullContainers = message.Health / HealthPerContainer;
      var partialHealth = message.Health % HealthPerContainer;

      foreach (var container in HealthContainers)
      {
        if (fullContainers > 0)
          container.sprite = this.heartImages[HealthPerContainer];
        else if (fullContainers == 0)
          container.sprite = this.heartImages[partialHealth];
        else
          container.sprite = this.heartImages.First();

        fullContainers--;
      }
    }
  }
}
