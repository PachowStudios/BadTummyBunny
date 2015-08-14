using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/HUD/Health")]
public class HealthHud : MonoBehaviour
{
	#region Inspector Fields
	public Image heartPrefab;
	public float spaceBetweenHearts = 0.125f;
	[Tooltip("Order is empty to full")]
	public Sprite[] heartImages = new Sprite[PlayerHealth.HealthPerContainer + 1];
	#endregion

	#region Internal Fields
	private List<Image> heartContainers;
	#endregion

	#region Public Properties
	public static HealthHud Instance { get; private set; }
	#endregion

	#region MonoBehaviour
	private void Awake()
	{
		Instance = this;

		heartContainers = transform.GetComponentsInChildren<Image>().OrderBy(h => h.rectTransform.anchoredPosition.x).ToList();
	}

	private void Start()
	{
		PlayerHealth.Instance.OnHeartContainersChanged += UpdateHeartContainers;
		PlayerHealth.Instance.OnHealthChanged += UpdateHealth;

		UpdateHeartContainers(PlayerHealth.Instance.HeartContainers);
		UpdateHealth(PlayerHealth.Instance.Health);
	}

	private void OnDisable()
	{
		PlayerHealth.Instance.OnHeartContainersChanged -= UpdateHeartContainers;
		PlayerHealth.Instance.OnHealthChanged -= UpdateHealth;
	}
	#endregion

	#region Internal Helper Methods
	private void UpdateHeartContainers(int newHeartContainers)
	{
		if (heartContainers.Count < newHeartContainers)
		{
			while (heartContainers.Count < newHeartContainers)
			{
				var newHeart = Instantiate(heartPrefab, Vector3.zero, Quaternion.identity) as Image;

				newHeart.transform.SetParent(transform, false);
				newHeart.sprite = heartImages[0];

				if (heartContainers.Count > 0)
					newHeart.rectTransform.anchoredPosition = new Vector2(heartContainers.Last().rectTransform.offsetMax.x + spaceBetweenHearts,
																																heartContainers.Last().rectTransform.anchoredPosition.y);

				heartContainers.Add(newHeart);
			}
		}
		else if (heartContainers.Count > newHeartContainers)
		{
			while (heartContainers.Count > newHeartContainers)
				Destroy(heartContainers.Pop().gameObject);
		}
	}

	private void UpdateHealth(int newHealth)
	{
		var fullContainers = newHealth / PlayerHealth.HealthPerContainer;
		var partialHealth = newHealth % PlayerHealth.HealthPerContainer;

		foreach (var container in heartContainers)
		{
			if (fullContainers > 0)
				container.sprite = heartImages[PlayerHealth.HealthPerContainer];
			else if (fullContainers == 0)
				container.sprite = heartImages[partialHealth];
			else
				container.sprite = heartImages[0];

			fullContainers--;
		}
	}
	#endregion
}
