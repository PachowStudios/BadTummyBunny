using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class SpriteExplosion : MonoBehaviour
{
	[SerializeField]
	private float lifetime = 1f;
	[SerializeField]
	private float systemLifetime = 5f;
	[SerializeField]
	private string sortingLayer = "Foreground";
	[SerializeField]
	private int sortingOrder = 1;
	public Material material;

	private IEnumerator DoExplode(Vector3 velocity, Sprite sprite)
	{
		var partSystem = GetComponent<ParticleSystem>();
		var particles = new List<ParticleSystem.Particle>();
		var currentParticle = new ParticleSystem.Particle();

		partSystem.renderer.sortingLayerName = sortingLayer;
		partSystem.renderer.sortingOrder = sortingOrder;
		currentParticle.size = 1f / sprite.pixelsPerUnit;

		var randomTranslate = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

		if (material != null)
			partSystem.renderer.material = material;

		for (int i = 0; i < sprite.bounds.size.x * sprite.pixelsPerUnit; i++)
		{
			for (int j = 0; j < sprite.bounds.size.y * sprite.pixelsPerUnit; j++)
			{
				Vector2 positionOffset = new Vector2(sprite.bounds.extents.x - sprite.bounds.center.x - 0.05f,
																						 sprite.bounds.extents.y - sprite.bounds.center.y - 0.05f);

				Vector3 particlePosition = transform.TransformPoint((i / sprite.pixelsPerUnit) - positionOffset.x,
																														(j / sprite.pixelsPerUnit) - positionOffset.y, 0);

				Color particleColor = sprite.texture.GetPixel((int)sprite.rect.x + i,
																											(int)sprite.rect.y + j);

				if (particleColor.a != 0f)
				{
					currentParticle.position = particlePosition;
					currentParticle.rotation = 0f;
					currentParticle.color = particleColor;
					currentParticle.startLifetime = currentParticle.lifetime = lifetime;

					currentParticle.velocity = new Vector2(velocity.x + Random.Range(-3f, 3f), 
																								 velocity.y + Random.Range(-3f, 3f));

					currentParticle.velocity += randomTranslate.ToVector3();

					particles.Add(currentParticle);
				}
			}
		}

		partSystem.SetParticles(particles.ToArray(), particles.Count);
		Destroy(gameObject, systemLifetime);

		yield return null;
	}

	public void Explode(Vector3 velocity, Sprite sprite) => StartCoroutine(DoExplode(velocity, sprite));
}
