using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Vectrosity;
using Zenject;

namespace BadTummyBunny
{
  [AddComponentMenu("Bad Tummy Bunny/Player/Farts/Basic Fart")]
  [RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
  public class BasicFart : MonoBehaviour, IFart
  {
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    protected struct SfxPowerMapping
    {
      [UsedImplicitly] public SfxGroups sfxGroup;
      [UsedImplicitly] public float power;
    }

    [Header("Options")]
    [SerializeField] private string fartName = "Basic Fart";
    [SerializeField] private Vector2 speedRange = default(Vector2);
    [SerializeField] private int damage = 4;
    [SerializeField] private float damageDelay = 0.1f;
    [SerializeField] private Vector2 knockback = new Vector2(1f, 1f);
    [SerializeField] private List<SfxPowerMapping> soundEffects = new List<SfxPowerMapping>();

    [Header("Trajectory")]
    [SerializeField] private float trajectoryPreviewTime = 1f;
    [SerializeField] private float trajectoryStartDistance = 1f;
    [SerializeField] private float trajectoryWidth = 0.3f;
    [SerializeField, Range(8, 64)] private int trajectorySegments = 16;
    [SerializeField] private Gradient trajectoryGradient = default(Gradient);
    [SerializeField] private Material trajectoryMaterial = null;
    [SerializeField] private string trajectorySortingLayer = "UI";
    [SerializeField] private int trajectorySortingOrder = -1;

    [Header("Components")]
    [SerializeField] private PolygonCollider2D fartCollider = null;
    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();

    private HashSet<ICharacter> pendingTargets = new HashSet<ICharacter>();
    private HashSet<ICharacter> damagedEnemies = new HashSet<ICharacter>();
    private VectorLine trajectoryLine;

    [Inject(Tags.Player)]
    private IMovable PlayerMovement { get; set; }

    [Inject]
    private CameraController CameraController { get; set; }

    public string FartName => this.fartName;

    public bool IsFarting { get; protected set; }

    [PostInject]
    private void PostInject()
      => InitializeTrajectoryLine();

    protected virtual void OnDestroy()
      => VectorLine.Destroy(ref this.trajectoryLine);

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
      if (!IsFarting)
        return;

      if (other.tag == Tags.Enemy)
        OnEnemyTriggered(other.GetInterface<ICharacter>());
    }

    public virtual void StartFart(float power, Vector2 direction)
    {
      if (IsFarting)
        return;

      IsFarting = true;

      PlaySound(power.Clamp01());
      Wait.ForFixedUpdate(StartParticles);
    }

    public virtual void StopFart()
    {
      if (!IsFarting)
        return;

      IsFarting = false;

      this.pendingTargets.Clear();
      this.damagedEnemies.Clear();
      this.particles.ForEach(p => p.Stop());
    }

    public virtual float CalculateSpeed(float power)
      => Extensions.ConvertRange(power, 0f, 1f, this.speedRange.x, this.speedRange.y);

    public virtual void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition)
    {
      var points = CalculateTrajectory(power, direction, gravity, startPosition);

      if (points != null)
      {
        this.trajectoryLine.SetColor(this.trajectoryGradient.Evaluate(power));
        this.trajectoryLine.MakeSpline(points);
        this.trajectoryLine.Draw();
      }
      else
        ClearTrajectory();
    }

    public virtual void ClearTrajectory()
    {
      this.trajectoryLine.SetColor(Color.clear);
      this.trajectoryLine.Draw();
    }

    protected virtual void OnEnemyTriggered(ICharacter enemy)
    {
      if (this.pendingTargets.Contains(enemy) || this.damagedEnemies.Contains(enemy))
        return;

      this.pendingTargets.Add(enemy);
      Wait.ForSeconds(
        this.damageDelay,
        () =>
        {
          this.pendingTargets.Remove(enemy);
          TryDamageEnemy(enemy);
        });
    }

    protected virtual void TryDamageEnemy(ICharacter enemy)
    {
      if (this.damagedEnemies.Contains(enemy))
        return;

      var origin = this.fartCollider.transform.position;

      if (this.fartCollider.OverlapPoint(enemy.Movement.CenterPoint) &&
          Physics2D.Linecast(origin, enemy.Movement.CenterPoint, PlayerMovement.CollisionLayers).collider == null)
      {
        this.damagedEnemies.Add(enemy);
        DamageEnemy(enemy);
      }
    }

    protected virtual void DamageEnemy(ICharacter enemy)
      => enemy.Health.Damage(this.damage, this.knockback, PlayerMovement.MovementDirection.Dot(-1f, 1f));

    protected virtual Vector3[] CalculateTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition)
    {
      if (power <= 0f)
        return null;

      var points = new Vector3[this.trajectorySegments];
      var speed = CalculateSpeed(power);
      var velocity = direction * speed;
      var timeStep = this.trajectoryPreviewTime / this.trajectorySegments;
      var bufferDelta = direction * this.trajectoryStartDistance;

      gravity *= timeStep * 0.5f;
      bufferDelta.y += gravity * Mathf.Pow(this.trajectoryStartDistance / speed, 2) * 0.5f;

      var bufferSqrMagnitude = bufferDelta.sqrMagnitude;

      for (var i = 0; i < this.trajectorySegments; i++)
      {
        var currentDelta = velocity;

        currentDelta.y += gravity * i;
        currentDelta *= timeStep * i;

        if (bufferSqrMagnitude > currentDelta.sqrMagnitude)
          points[i] = startPosition + bufferDelta;
        else
          points[i] = startPosition + currentDelta;

        if (i <= 0)
          continue;

        var linecast =
          Physics2D.Linecast(
            points[i - 1],
            points[i],
            PlayerMovement.CollisionLayers);

        if (linecast.collider == null)
          continue;

        for (var j = i - 1; j < this.trajectorySegments; j++)
          points[j] = linecast.point;

        break;
      }

      return points;
    }

    protected void PlaySound(float powerPercentage)
    {
      foreach (var sfxMapping in this.soundEffects.Where(e => e.power <= powerPercentage))
      {
        SoundManager.PlayCappedSFXFromGroup(sfxMapping.sfxGroup);
        break;
      }
    }

    protected void StartParticles()
      => this.particles.ForEach(p => p.Play());

    protected void InitializeTrajectoryLine()
    {
      VectorLine.SetCanvasCamera(CameraController.Camera);
      VectorLine.canvas.planeDistance = 9;
      VectorLine.canvas.sortingLayerName = this.trajectorySortingLayer;
      VectorLine.canvas.sortingOrder = this.trajectorySortingOrder;

      this.trajectoryLine =
        new VectorLine(
          "Trajectory",
          new List<Vector3>(this.trajectorySegments),
          Extensions.UnitsToPixels(this.trajectoryWidth),
          LineType.Continuous,
          Joins.Fill)
        {
          material = this.trajectoryMaterial,
          textureScale = 1f
        };
    }
  }
}
