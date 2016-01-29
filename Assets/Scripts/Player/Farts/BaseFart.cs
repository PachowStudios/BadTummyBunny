using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using UnityEngine;
using Vectrosity;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class BaseFart<TConfig> : IFart,
    IHandles<FartEnemyTriggeredMessage>
      where TConfig : FartSettings
  {
    private VectorLine trajectoryLine;

    [Inject] private FartView View { get; set; }
    [Inject] private CameraController CameraController { get; set; }
    [Inject] private IEventAggregator EventAggregator { get; set; }

    private IMovable PlayerMovement { get; set; }

    protected abstract TConfig Config { get; set; }

    protected HashSet<ICharacter> PendingTargets { get; } = new HashSet<ICharacter>();
    protected HashSet<ICharacter> DamagedEnemies { get; } = new HashSet<ICharacter>();

    public bool IsFarting { get; protected set; }

    public bool ShowTrajectory
    {
      get { return TrajectoryLine.active; }
      set { TrajectoryLine.active = value; }
    }

    protected VectorLine TrajectoryLine => this.trajectoryLine;
    protected List<Vector2> TrajectoryPoints => TrajectoryLine.points2;

    public string Name => Config.Name;
    public FartType Type => Config.Type;

    [PostInject]
    private void Initialize()
    {
      EventAggregator.Subscribe(this);
      InitializeTrajectoryLine();
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

      PendingTargets.Clear();
      DamagedEnemies.Clear();
      View.Particles.ForEach(p => p.Stop());
    }

    public virtual float CalculateSpeed(float power)
      => MathHelper.ConvertRange(power, 0f, 1f, Config.SpeedRange.x, Config.SpeedRange.y);

    public virtual void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition)
    {
      if (!ShowTrajectory)
        return;

      UpdateTrajectoryPoints(power, direction, gravity, startPosition);
      TrajectoryLine.SetColor(Config.TrajectoryGradient.Evaluate(power));
      TrajectoryLine.Draw();
    }

    protected virtual void TryDamageEnemy(IEnemy enemy)
    {
      if (DamagedEnemies.Contains(enemy))
        return;

      var origin = View.FartCollider.transform.position;

      if (View.FartCollider.OverlapPoint(enemy.Movement.CenterPoint)
          && Physics2D.Linecast(origin, enemy.Movement.CenterPoint, PlayerMovement.CollisionLayers).collider == null)
      {
        DamagedEnemies.Add(enemy);
        DamageEnemy(enemy);
      }
    }

    protected virtual void DamageEnemy(IEnemy enemy)
      => enemy.Health.Damage(
        Config.Damage,
        Config.Knockback,
        PlayerMovement.MovementDirection.Dot(-1f, 1f));

    protected virtual void UpdateTrajectoryPoints(float power, Vector3 direction, float gravity, Vector3 startPosition)
    {
      TrajectoryPoints.Clear();

      var speed = CalculateSpeed(power);
      var velocity = direction * speed;
      var timeStep = Config.TrajectoryPreviewTime / Config.TrajectorySegments;
      var bufferDelta = direction * Config.TrajectoryStartDistance;

      gravity *= timeStep * 0.5f;
      bufferDelta.y += gravity * (Config.TrajectoryStartDistance / speed).Square() * 0.5f;

      var bufferSqrMagnitude = bufferDelta.sqrMagnitude;

      for (var i = 0; i < Config.TrajectorySegments; i++)
      {
        var currentDelta = velocity;

        currentDelta.y += gravity * i;
        currentDelta *= timeStep * i;

        if (bufferSqrMagnitude > currentDelta.sqrMagnitude)
          continue;

        TrajectoryPoints.Add(startPosition + currentDelta);

        if (TrajectoryPoints.HasMultiple()
            && Physics2D.Linecast(TrajectoryPoints.ElementsBeforeLast(1), TrajectoryPoints.Last(), PlayerMovement.CollisionLayers).collider != null)
          break;
      }

      for (var i = 0; i < TrajectoryPoints.Count; i++)
        TrajectoryPoints[i] = CameraController.Camera.WorldToScreenPoint(TrajectoryPoints[i]);
    }

    protected void PlaySound(float powerPercentage)
      => SoundManager.PlayCappedSFXFromGroup(
        Config.SoundEffects
          .Where(e => e.Power <= powerPercentage)
          .Highest(e => e.Power)
          .SfxGroup);

    protected void StartParticles()
      => View.Particles.ForEach(p => p.Play());

    protected void InitializeTrajectoryLine()
    {
      VectorLine.canvas.planeDistance = 9;
      VectorLine.canvas.sortingLayerName = Config.TrajectorySortingLayer;
      VectorLine.canvas.sortingOrder = Config.TrajectorySortingOrder;

      this.trajectoryLine =
        new VectorLine(
          "Trajectory",
          new List<Vector2>(Config.TrajectorySegments),
          CameraController.Camera.UnitsToPixels(Config.TrajectoryWidth),
          LineType.Points)
        {
          //material = Config.TrajectoryMaterial,
          //textureScale = 1f
        };
    }

    public void Handle(FartEnemyTriggeredMessage message)
    {
      var enemy = message.Enemy;

      if (!IsFarting
          || PendingTargets.Contains(enemy)
          || DamagedEnemies.Contains(enemy))
        return;

      PendingTargets.Add(enemy);
      Wait.ForSeconds(
        Config.DamageDelay,
        () =>
        {
          PendingTargets.Remove(enemy);
          TryDamageEnemy(enemy);
        });
    }
  }
}
