﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using UnityEngine;
using Vectrosity;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class Fart : IFart
  {
    [InstallerSettings]
    public class Settings : ScriptableObject
    {
      [Serializable, InstallerSettings]
      public struct SfxPowerMapping
      {
        public SfxGroup SfxGroup;
        public float Power;
      }

      [Header("Definition")]
      public string Name = "Basic Fart";
      public FartType Type;
      public GameObject Prefab;

      [Header("Options")]
      public Vector2 SpeedRange = default(Vector2);
      public int Damage = 4;
      public float DamageDelay = 0.1f;
      public Vector2 Knockback = new Vector2(1f, 1f);
      public List<SfxPowerMapping> SoundEffects = new List<SfxPowerMapping>();

      [Header("Trajectory")]
      public float TrajectoryPreviewTime = 1f;
      public float TrajectoryStartDistance = 1f;
      public float TrajectoryWidth = 0.3f;
      [Range(8, 64)] public int TrajectorySegments = 16;
      public Gradient TrajectoryGradient = default(Gradient);
      public Material TrajectoryMaterial = null;
      public string TrajectorySortingLayer = "UI";
      public int TrajectorySortingOrder = -1;
    }

    private readonly HashSet<ICharacter> pendingTargets = new HashSet<ICharacter>();
    private readonly HashSet<ICharacter> damagedEnemies = new HashSet<ICharacter>();

    private VectorLine trajectoryLine;

    [InjectLocal] private Settings Config { get; set; }
    [InjectLocal] private FartView View { get; set; }

    [Inject] private Player Player { get; set; }
    [Inject] private CameraController CameraController { get; set; }

    public bool IsFarting { get; protected set; }

    public string Name => Config.Name;
    public FartType Type => Config.Type;

    [PostInject]
    private void Initialize()
      => InitializeTrajectoryLine();

    public void Attach(PlayerView playerView)
      => View.Attach(playerView);

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

      this.pendingTargets.Clear();
      this.damagedEnemies.Clear();
      View.Particles.ForEach(p => p.Stop());
    }

    public virtual float CalculateSpeed(float power)
      => MathHelper.ConvertRange(power, 0f, 1f, Config.SpeedRange.x, Config.SpeedRange.y);

    public virtual void DrawTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition)
    {
      var points = CalculateTrajectory(power, direction, gravity, startPosition);

      if (points != null)
      {
        this.trajectoryLine.SetColor(Config.TrajectoryGradient.Evaluate(power));
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

    protected virtual void TryDamageEnemy(IEnemy enemy)
    {
      if (this.damagedEnemies.Contains(enemy))
        return;

      var origin = View.FartCollider.transform.position;

      if (View.FartCollider.OverlapPoint(enemy.Movement.CenterPoint)
          && Physics2D.Linecast(origin, enemy.Movement.CenterPoint, Player.Movement.CollisionLayers).collider == null)
      {
        this.damagedEnemies.Add(enemy);
        DamageEnemy(enemy);
      }
    }

    protected virtual void DamageEnemy(IEnemy enemy)
      => enemy.Health.Damage(
        Config.Damage,
        Config.Knockback,
        Player.Movement.MovementDirection.Dot(-1f, 1f));

    protected virtual Vector3[] CalculateTrajectory(float power, Vector3 direction, float gravity, Vector3 startPosition)
    {
      if (power <= 0f)
        return null;

      var points = new Vector3[Config.TrajectorySegments];
      var speed = CalculateSpeed(power);
      var velocity = direction * speed;
      var timeStep = Config.TrajectoryPreviewTime / Config.TrajectorySegments;
      var bufferDelta = direction * Config.TrajectoryStartDistance;

      gravity *= timeStep * 0.5f;
      bufferDelta.y += gravity * Mathf.Pow(Config.TrajectoryStartDistance / speed, 2) * 0.5f;

      var bufferSqrMagnitude = bufferDelta.sqrMagnitude;

      for (var i = 0; i < Config.TrajectorySegments; i++)
      {
        var currentDelta = velocity;

        currentDelta.y += gravity * i;
        currentDelta *= timeStep * i;

        points[i] = bufferSqrMagnitude > currentDelta.sqrMagnitude
          ? startPosition + bufferDelta
          : startPosition + currentDelta;

        if (i <= 0)
          continue;

        var linecast =
          Physics2D.Linecast(
            points[i - 1],
            points[i],
            Player.Movement.CollisionLayers);

        if (linecast.collider == null)
          continue;

        for (var j = i - 1; j < Config.TrajectorySegments; j++)
          points[j] = linecast.point;

        break;
      }

      return points;
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
      VectorLine.SetCanvasCamera(CameraController.Camera);
      VectorLine.canvas.planeDistance = 9;
      VectorLine.canvas.sortingLayerName = Config.TrajectorySortingLayer;
      VectorLine.canvas.sortingOrder = Config.TrajectorySortingOrder;

      this.trajectoryLine =
        new VectorLine(
          "Trajectory",
          new List<Vector3>(Config.TrajectorySegments),
          CameraController.Camera.UnitsToPixels(Config.TrajectoryWidth),
          LineType.Continuous,
          Joins.Fill)
        {
          material = Config.TrajectoryMaterial,
          textureScale = 1f
        };
    }

    public virtual void OnEnemyTriggered(IEnemy enemy)
    {
      if (this.pendingTargets.Contains(enemy) || this.damagedEnemies.Contains(enemy))
        return;

      this.pendingTargets.Add(enemy);
      Wait.ForSeconds(
        Config.DamageDelay,
        () =>
        {
          this.pendingTargets.Remove(enemy);
          TryDamageEnemy(enemy);
        });
    }
  }
}