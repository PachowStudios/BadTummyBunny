using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour, ICharacter
{
	public virtual IMovable Movement { get; }
	public virtual IHasHealth Health { get; }
}