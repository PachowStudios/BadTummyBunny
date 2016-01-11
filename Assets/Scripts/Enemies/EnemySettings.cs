using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [InstallerSettings, CreateAssetMenu(menuName = "Bad Tummy Bunny/Enemies/Enemy Settings")]
  public class EnemySettings : ScriptableObject
  {
    [Header("Definition")]
    public string Name = "New Enemy";
    public EnemyView Prefab;

    [Header("Options")]
    public int ContactDamage = 1;
    public Vector2 ContactKnockback = new Vector2(2f, 1f);

    [Header("Component Settings")]
    public EnemyMovementSettings Movement;
    public EnemyHealthSettings Health;
  }
}