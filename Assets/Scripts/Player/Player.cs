namespace PachowStudios.BadTummyBunny
{
  public sealed class Player : StatusEffectableCharacter
  {
    public PlayerMovement Movement { get; }
    public PlayerHealth Health { get; }

    public IFartInfoProvider FartInfo => Movement;

    public Player(IView view, PlayerMovement movement, PlayerHealth health)
      : base(view, movement, health)
    {
      Movement = movement;
      Health = health;
    }
  }
}
