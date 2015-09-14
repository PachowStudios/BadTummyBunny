using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/HUD/Health")]
public class HealthHud : MonoBehaviour
{
	[SerializeField]
	protected Image heartPrefab;
	[SerializeField]
	protected float spaceBetweenHearts = 0.125f;
	[Tooltip("Order is empty to full")]
	[SerializeField]
	protected Sprite[] heartImages = new Sprite[5];

	protected List<Image> healthContainers = null;

	public static HealthHud Instance { get; private set; }

	protected int HealthPerContainer => Player.Instance.HealthContainers.HealthPerContainer;

	protected virtual void Awake()
	{
		Instance = this;

		healthContainers = transform.GetComponentsInChildren<Image>()
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
		if (healthContainers.Count < newHealthContainers)
		{
			while (healthContainers.Count < newHealthContainers)
			{
				var newHeart = Instantiate(heartPrefab, Vector3.zero, Quaternion.identity) as Image;

				newHeart.transform.SetParent(transform, false);
				newHeart.sprite = heartImages[0];

				if (healthContainers.Count > 0)
					newHeart.rectTransform.anchoredPosition = new Vector2(healthContainers.Last().rectTransform.offsetMax.x + spaceBetweenHearts,
																																healthContainers.Last().rectTransform.anchoredPosition.y);

				healthContainers.Add(newHeart);
			}
		}
		else if (healthContainers.Count > newHealthContainers)
		{
			while (healthContainers.Count > newHealthContainers)
				Destroy(healthContainers.Pop().gameObject);
		}
	}

	protected virtual void OnHealthChanged(int newHealth)
	{
		var fullContainers = newHealth / HealthPerContainer;
		var partialHealth = newHealth % HealthPerContainer;

		foreach (var container in healthContainers)
		{
			if (fullContainers > 0)
				container.sprite = heartImages[HealthPerContainer];
			else if (fullContainers == 0)
				container.sprite = heartImages[partialHealth];
			else
				container.sprite = heartImages[0];

			fullContainers--;
		}
	}
}
