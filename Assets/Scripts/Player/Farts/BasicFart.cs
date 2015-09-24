using System;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

[AddComponentMenu("Player/Farts/Basic Fart")]
[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class BasicFart : MonoBehaviour, IFart
{
	[Serializable]
	protected struct SfxPowerMapping
	{
		public SfxGroups sfxGroup;
		public float power;
	}

	[Header("Options")]
	[SerializeField]
	private string fartName = "Basic Fart";
	[SerializeField]
	protected Vector2 speedRange = default(Vector2);
	[SerializeField]
	protected int damage = 4;
	[SerializeField]
	protected float damageDelay = 0.1f;
	[SerializeField]
	protected Vector2 knockback = new Vector2(1f, 1f);
	[SerializeField]
	protected List<SfxPowerMapping> soundEffects = new List<SfxPowerMapping>();

	[Header("Trajectory")]
	[SerializeField]
	protected float trajectoryPreviewTime = 1f;
	[SerializeField]
	protected float trajectoryStartDistance = 1f;
	[SerializeField]
	protected float trajectoryWidth = 0.3f;
	[SerializeField]
	[Range(8, 64)]
	protected int trajectorySegments = 16;
	[SerializeField]
	protected Gradient trajectoryGradient = default(Gradient);
	[SerializeField]
	protected Material trajectoryMaterial = null;
	[SerializeField]
	protected string trajectorySortingLayer = "UI";
	[SerializeField]
	protected int trajectorySortingOrder = -1;

	[Header("Components")]
	[SerializeField]
	protected PolygonCollider2D fartCollider = null;
	[SerializeField]
	protected List<ParticleSystem> particles = new List<ParticleSystem>();

	private HashSet<ICharacter> pendingTargets = new HashSet<ICharacter>();
	private HashSet<ICharacter> damagedEnemies = new HashSet<ICharacter>();
	private VectorLine trajectoryLine = null;

	public string FartName => fartName;

	public bool IsFarting { get; protected set; }

	protected virtual void Awake() 
		=> InitializeTrajectoryLine();

	protected virtual void OnDestroy() 
		=> VectorLine.Destroy(ref trajectoryLine);

	public virtual void StartFart(float power, Vector2 direction)
	{
		if (IsFarting)
			return;

		IsFarting = true;

		PlaySound(power.Clamp01());
		Wait.ForFixedUpdate(StartParticles);
	}

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		if (!IsFarting)
			return;

		if (other.tag == Tags.Enemy)
		{
			var enemy = other.GetInterface<ICharacter>();

			if (pendingTargets.Contains(enemy) ||
					damagedEnemies.Contains(enemy))
				return;

			pendingTargets.Add(enemy);
			Wait.ForSeconds(damageDelay,
				() =>
				{
					pendingTargets.Remove(enemy);
					TryDamageEnemy(enemy);
				});
		}
	}

	public virtual void StopFart()
	{
		if (!IsFarting)
			return;

		IsFarting = false;

		pendingTargets.Clear();
		damagedEnemies.Clear();
		particles.ForEach(p => p.Stop());
	}

	public virtual float CalculateSpeed(float power) 
		=> Extensions.ConvertRange(power, 0f, 1f, speedRange.x, speedRange.y);

	public virtual void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition)
	{
		var points = CalculateTrajectory(power, direction, gravity, startPosition);

		if (points != null)
		{
			trajectoryLine.SetColor(trajectoryGradient.Evaluate(power));
			trajectoryLine.MakeSpline(points);
			trajectoryLine.Draw();
		}
		else
			ClearTrajectory();
	}

	public virtual void ClearTrajectory()
	{
		trajectoryLine.SetColor(Color.clear);
		trajectoryLine.Draw();
	}

	protected virtual void TryDamageEnemy(ICharacter enemy)
	{
		if (damagedEnemies.Contains(enemy))
			return;

		var origin = fartCollider.transform.position;

		if (fartCollider.OverlapPoint(enemy.Movement.CenterPoint) && 
				Physics2D.Linecast(origin, enemy.Movement.CenterPoint, Player.Instance.Movement.CollisionLayers).collider == null)
		{
			damagedEnemies.Add(enemy);
			DamageEnemy(enemy);
		}
	}

	protected virtual void DamageEnemy(ICharacter enemy) 
		=> enemy.Health.Damage(damage, knockback, Player.Instance.Movement.MovementDirection.Dot(-1f, 1f));

	protected virtual Vector3[] CalculateTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition)
	{
		if (power <= 0f) return null;

		var points = new Vector3[trajectorySegments];
		var speed = CalculateSpeed(power);
		var velocity = direction * speed;
		var timeStep = trajectoryPreviewTime / trajectorySegments;
		var bufferDelta = direction * trajectoryStartDistance;

		gravity *= timeStep * 0.5f;
		bufferDelta.y += gravity * Mathf.Pow((trajectoryStartDistance / speed), 2) * 0.5f;

		var bufferSqrMagnitude = bufferDelta.sqrMagnitude;

		for (int i = 0; i < trajectorySegments; i++)
		{
			var currentDelta = velocity;

			currentDelta.y += gravity * i;
			currentDelta *= timeStep * i;

			if (bufferSqrMagnitude > currentDelta.sqrMagnitude)
				points[i] = startPosition + bufferDelta;
			else
				points[i] = startPosition + currentDelta;

			if (i > 0)
			{
				var linecast = Physics2D.Linecast(points[i - 1], 
																					points[i], 
																					Player.Instance.Movement.CollisionLayers);

				if (linecast.collider != null)
				{
					for (int j = i - 1; j < trajectorySegments; j++)
						points[j] = linecast.point;

					break;
				}
			}
		}

		return points;
	}

	protected void PlaySound(float powerPercentage)
	{
		foreach (var sfxMapping in soundEffects)
		{
			if (sfxMapping.power <= powerPercentage)
			{
				SoundManager.PlayCappedSFXFromGroup(sfxMapping.sfxGroup);
				break;
			}
		}
	}

	protected void StartParticles() 
		=> particles.ForEach(p => p.Play());

	protected void InitializeTrajectoryLine()
	{
		VectorLine.SetCanvasCamera(CameraController.Instance.camera);
		VectorLine.canvas.planeDistance = 9;
		VectorLine.canvas.sortingLayerName = trajectorySortingLayer;
		VectorLine.canvas.sortingOrder = trajectorySortingOrder;

		trajectoryLine = new VectorLine("Trajectory",
																		new Vector3[trajectorySegments],
																		trajectoryMaterial,
																		Extensions.UnitsToPixels(trajectoryWidth),
																		LineType.Continuous,
																		Joins.Fill);
		trajectoryLine.textureScale = 1f;
	}
}