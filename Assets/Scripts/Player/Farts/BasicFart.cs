using System;
using System.Collections.Generic;
using System.Linq;
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
  [SerializeField] private string fartName = "Basic Fart";
  [SerializeField] protected Vector2 speedRange = default(Vector2);
  [SerializeField] protected int damage = 4;
  [SerializeField] protected float damageDelay = 0.1f;
  [SerializeField] protected Vector2 knockback = new Vector2(1f, 1f);
  [SerializeField] protected List<SfxPowerMapping> soundEffects = new List<SfxPowerMapping>();

  [Header("Trajectory")]
  [SerializeField] protected float trajectoryPreviewTime = 1f;
  [SerializeField] protected float trajectoryStartDistance = 1f;
  [SerializeField] protected float trajectoryWidth = 0.3f;
  [SerializeField, Range(8, 64)]  protected int trajectorySegments = 16;
  [SerializeField] protected Gradient trajectoryGradient = default(Gradient);
  [SerializeField] protected Material trajectoryMaterial = null;
  [SerializeField] protected string trajectorySortingLayer = "UI";
  [SerializeField] protected int trajectorySortingOrder = -1;

  [Header("Components")]
  [SerializeField] protected PolygonCollider2D fartCollider = null;
  [SerializeField] protected List<ParticleSystem> particles = new List<ParticleSystem>();

  private HashSet<ICharacter> pendingTargets = new HashSet<ICharacter>();
  private HashSet<ICharacter> damagedEnemies = new HashSet<ICharacter>();
  private VectorLine trajectoryLine;

  public string FartName => this.fartName;

  public bool IsFarting { get; protected set; }

  protected virtual void Awake()
    => InitializeTrajectoryLine();

  protected virtual void OnDestroy()
    => VectorLine.Destroy(ref this.trajectoryLine);

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

      if (this.pendingTargets.Contains(enemy) || this.damagedEnemies.Contains(enemy))
        return;

      this.pendingTargets.Add(enemy);
      Wait.ForSeconds(this.damageDelay,
                      () =>
                      {
                        this.pendingTargets.Remove(enemy);
                        TryDamageEnemy(enemy);
                      });
    }
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

  protected virtual void TryDamageEnemy(ICharacter enemy)
  {
    if (this.damagedEnemies.Contains(enemy))
      return;

    var origin = this.fartCollider.transform.position;

    if (this.fartCollider.OverlapPoint(enemy.Movement.CenterPoint) &&
        Physics2D.Linecast(origin, enemy.Movement.CenterPoint, Player.Instance.Movement.CollisionLayers).collider == null)
    {
      this.damagedEnemies.Add(enemy);
      DamageEnemy(enemy);
    }
  }

  protected virtual void DamageEnemy(ICharacter enemy)
    => enemy.Health.Damage(this.damage, this.knockback, Player.Instance.Movement.MovementDirection.Dot(-1f, 1f));

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
    bufferDelta.y += gravity * Mathf.Pow((this.trajectoryStartDistance / speed), 2) * 0.5f;

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

      var linecast = Physics2D.Linecast(points[i - 1],
                                        points[i],
                                        Player.Instance.Movement.CollisionLayers);

      if (linecast.collider != null)
      {
        for (var j = i - 1; j < this.trajectorySegments; j++)
          points[j] = linecast.point;

        break;
      }
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
    VectorLine.SetCanvasCamera(CameraController.Instance.Camera);
    VectorLine.canvas.planeDistance = 9;
    VectorLine.canvas.sortingLayerName = this.trajectorySortingLayer;
    VectorLine.canvas.sortingOrder = this.trajectorySortingOrder;

    this.trajectoryLine = new VectorLine("Trajectory",
                                         new Vector3[this.trajectorySegments], this.trajectoryMaterial,
                                         Extensions.UnitsToPixels(this.trajectoryWidth),
                                         LineType.Continuous,
                                         Joins.Fill)
                          {
                            textureScale = 1f
                          };
  }
}
