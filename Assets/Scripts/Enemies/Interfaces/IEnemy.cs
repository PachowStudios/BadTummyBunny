using UnityEngine;

public interface IEnemy : ICharacter
{
	int ContactDamage { get; }
	Vector2 ContactKnockback { get; }
}