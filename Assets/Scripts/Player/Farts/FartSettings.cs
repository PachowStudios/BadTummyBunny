using System;
using System.Collections.Generic;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Farts/Fart Settings")]
  public class FartSettings : ScriptableObject
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
    public FartView Prefab;

    [Header("Options")]
    public Vector2 SpeedRange = default(Vector2);
    public int Damage = 4;
    public float DamageDelay = 0.1f;
    public Vector2 Knockback = new Vector2(1f, 1f);
    public List<SfxPowerMapping> SoundEffects = new List<SfxPowerMapping>();

    [Header("Trajectory")]
    public float TrajectoryPreviewTime = 1f;
    public float TrajectoryStartDistance = 1f;
    public float TrajectoryPointSeparation = 0.5f;
    public float TrajectoryPointSize = 0.125f;
    [Range(8, 64)]
    public int TrajectorySegments = 16;
    public Gradient TrajectoryGradient = null;
    public string TrajectorySortingLayer = "UI";
    public int TrajectorySortingOrder = -1;
  }
}