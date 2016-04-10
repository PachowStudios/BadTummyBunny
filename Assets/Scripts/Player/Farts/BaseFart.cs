using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using PachowStudios.Assertions;
using UnityEngine;
using Vectrosity;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseFart : IFart,
    IHandles<FartEnemyTriggeredMessage>
  {
    private VectorLine trajectoryLine;
    private bool isFarting;
    private bool isSecondaryFarting;

    public bool IsFarting
    {
      get { return this.isFarting; }
      private set
      {
        this.isFarting = value;
        View.CanHitEnemies = value;
      }
    }

    public bool IsSecondaryFarting
    {
      get { return this.isSecondaryFarting; }
      private set
      {
        this.isSecondaryFarting = value;
        View.CanHitEnemies = value;
      }
    }

    public float TimeFarting { get; private set; }

    public bool ShowTrajectory
    {
      get { return TrajectoryLine.active; }
      set { TrajectoryLine.active = value; }
    }

    public string Name => Config.Name;
    public FartType Type => Config.Type;

    public bool CanFart => !IsFarting && !IsSecondaryFarting;

    protected HashSet<ICharacter> PendingTargets { get; } = new HashSet<ICharacter>();
    protected HashSet<ICharacter> DamagedEnemies { get; } = new HashSet<ICharacter>();

    protected VectorLine TrajectoryLine => this.trajectoryLine;
    protected List<Vector2> TrajectoryPoints => TrajectoryLine.points2;

    private FartSettings Config { get; }
    private FartView View { get; }

    [Inject] private Camera Camera { get; set; }
    [Inject] private IEventAggregator EventAggregator { get; set; }

    private IMovable PlayerMovement { get; set; }

    protected BaseFart(FartSettings config, FartView view)
    {
      Config = config;
      View = view;
    }

    [PostInject]
    private void Initialize()
    {
      EventAggregator.Subscribe(this);
      InitializeTrajectoryLine();
    }

    public void Tick()
    {
      if (IsFarting)
        TimeFarting += Time.deltaTime;
    }

    public void Attach(PlayerView playerView)
    {
      PlayerMovement = playerView.Model.Movement;
      View.Attach(playerView);
    }

    public void Detach()
    {
      VectorLine.Destroy(ref this.trajectoryLine);
      View.Detach();
    }

    public virtual void StartFarting(float power)
    {
      if (!CanFart)
        return;

      IsFarting = true;

      PlaySound(power);
      Wait.ForFixedUpdate(View.StartParticles);
    }

    public virtual void SecondaryFart(float power)
    {
      if (!CanFart)
        return;

      IsSecondaryFarting = true;

      PlaySound(power);
      Wait.ForFixedUpdate(View.StartParticles);
      Wait.ForSeconds(CalculateSecondaryDuration(power), StopFarting);
    }

    public virtual void StopFarting()
    {
      if (CanFart)
        return;

      IsFarting = IsSecondaryFarting = false;
      TimeFarting = 0f;
      PendingTargets.Clear();
      DamagedEnemies.Clear();
      View.StopParticles();
    }

    public virtual float CalculateSpeed(float power)
      => MathHelper.ConvertRange(power, 0f, 1f, Config.SpeedRange.x, Config.SpeedRange.y);

    public virtual void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition)
    {
      if (!ShowTrajectory)
        return;

      TrajectoryPoints.ReplaceAll(
        TranslateTrajectoryPoints(
          CalculateTrajectoryPoints(
            power, direction, gravity, startPosition)));

      TrajectoryLine.SetColor(CalculateTrajectoryColor(power));
      TrajectoryLine.Draw();
    }

    protected virtual void TargetEnemy(IEnemy enemy)
    {
      if (PendingTargets.Contains(enemy) || DamagedEnemies.Contains(enemy))
        return;

      PendingTargets.Add(enemy);
      Wait.ForSeconds(Config.DamageDelay, () => TryDamageEnemy(enemy));
    }

    protected virtual void TryDamageEnemy(IEnemy enemy)
    {
      DamagedEnemies.Should().NotContain(enemy, "because enemies cannot be damaged twice in one fart");

      PendingTargets.Remove(enemy);

      if (View.FartCollider.OverlapPoint(enemy.View.CenterPoint)
          && !Physics2D.Linecast(View.FartOrigin, enemy.View.CenterPoint, PlayerMovement.CollisionLayers))
      {
        DamagedEnemies.Add(enemy);
        DamageEnemy(enemy);
      }
    }

    protected virtual void DamageEnemy(IEnemy enemy)
    {
      enemy.Health.TakeDamage(
        Config.Damage,
        Config.Knockback,
        PlayerMovement.MovementDirection.Scale(x: -1f));

      if (enemy.Health.IsDead)
        EventAggregator.Publish(new PlayerKilledEnemyMessage(enemy));
    }

    private void PlaySound(float powerPercentage)
      => SoundManager.PlayCappedSFXFromGroup(Config.SoundEffects
        .Where(e => e.Power <= powerPercentage)
        .Highest(e => e.Power)
        .SfxGroup);

    private void InitializeTrajectoryLine()
    {
      VectorLine.canvas.planeDistance = 9;
      VectorLine.canvas.sortingLayerName = Config.TrajectorySortingLayer;
      VectorLine.canvas.sortingOrder = Config.TrajectorySortingOrder;

      this.trajectoryLine = new VectorLine(
        "Trajectory",
        new List<Vector2>(Config.TrajectorySegments),
        Camera.UnitsToPixels(Config.TrajectoryPointSize),
        LineType.Points);
    }

    private IEnumerable<Vector2> CalculateTrajectoryPoints(float power, Vector2 direction, float gravity, Vector2 startPosition)
    {
      var previous = startPosition;
      var speed = CalculateSpeed(power);
      var initialVelocity = direction * speed;
      var timeStep = Config.TrajectoryPreviewTime / Config.TrajectorySegments;
      var buffer = direction * Config.TrajectoryStartDistance;
      var hasPastBuffer = false;

      gravity *= timeStep / 2f;
      buffer.y += gravity * (Config.TrajectoryStartDistance / speed).Square() * 0.5f;

      var bufferSqrMagnitude = buffer.sqrMagnitude;

      for (var i = 0; i < Config.TrajectorySegments; i++)
      {
        var delta = initialVelocity;

        delta.y += gravity * i;
        delta *= timeStep * i;

        if (!hasPastBuffer)
        {
          hasPastBuffer = delta.sqrMagnitude >= bufferSqrMagnitude;

          if (hasPastBuffer)
            delta = buffer;
          else
            continue;
        }

        var current = startPosition + delta;

        if (Physics2D.Linecast(previous, current, PlayerMovement.CollisionLayers))
          break;

        yield return previous = current;
      }
    }

    private IEnumerable<Vector2> TranslateTrajectoryPoints(IEnumerable<Vector2> points)
    {
      using (var enumerator = points.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          yield break;

        var previous = enumerator.Current;

        yield return Camera.WorldToScreenPoint(previous);

        while (enumerator.MoveNext())
        {
          var distance = previous.DistanceTo(enumerator.Current);

          if (distance < Config.TrajectoryPointSeparation)
            continue;

          previous = previous.LerpTo(enumerator.Current, Config.TrajectoryPointSeparation / distance);

          yield return Camera.WorldToScreenPoint(previous);
        }
      }
    }

    private float CalculateSecondaryDuration(float power)
      => MathHelper.ConvertRange(power, 0f, 1f, Config.SecondaryDuration.x, Config.SecondaryDuration.y);

    private Color CalculateTrajectoryColor(float power)
      => Config.TrajectoryGradient.Evaluate(power);

    public void Handle(FartEnemyTriggeredMessage message)
      => TargetEnemy(message.Enemy);
  }
}
