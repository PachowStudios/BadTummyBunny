using InControl;

namespace PachowStudios.BadTummyBunny
{
  public class PlayerActions : PlayerActionSet
  {
    private readonly PlayerAction moveLeft;
    private readonly PlayerAction moveRight;

    private readonly PlayerAction fartLeft;
    private readonly PlayerAction fartRight;
    private readonly PlayerAction fartDown;
    private readonly PlayerAction fartUp;

    public PlayerOneAxisAction Move { get; }
    public PlayerTwoAxisAction Fart { get; }

    public PlayerAction Jump { get; }

    public PlayerActions()
    {
      this.moveLeft = CreatePlayerAction("Move Left");
      this.moveRight = CreatePlayerAction("Move Right");

      this.fartLeft = CreatePlayerAction("Fart Aim Left");
      this.fartRight = CreatePlayerAction("Fart Aim Right");
      this.fartDown = CreatePlayerAction("Fart Aim Down");
      this.fartUp = CreatePlayerAction("Fart Aim Up");

      Move = CreateOneAxisPlayerAction(this.moveLeft, this.moveRight);
      Fart = CreateTwoAxisPlayerAction(this.fartLeft, this.fartRight, this.fartDown, this.fartUp);

      Jump = CreatePlayerAction("Jump");
    }

    public static PlayerActions CreateWithDefaultBindings()
    {
      var playerActions = new PlayerActions();

      playerActions.moveLeft.AddDefaultBinding(Key.A);
      playerActions.moveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
      playerActions.moveLeft.AddDefaultBinding(InputControlType.DPadLeft);

      playerActions.moveRight.AddDefaultBinding(Key.D);
      playerActions.moveRight.AddDefaultBinding(InputControlType.LeftStickRight);
      playerActions.moveRight.AddDefaultBinding(InputControlType.DPadRight);

      playerActions.fartLeft.AddDefaultBinding(InputControlType.RightStickLeft);

      playerActions.fartRight.AddDefaultBinding(InputControlType.RightStickRight);

      playerActions.fartDown.AddDefaultBinding(InputControlType.RightStickDown);

      playerActions.fartUp.AddDefaultBinding(InputControlType.RightStickUp);

      playerActions.Jump.AddDefaultBinding(Key.Space);
      playerActions.Jump.AddDefaultBinding(InputControlType.Action1);

      playerActions.ListenOptions.IncludeUnknownControllers = false;
      playerActions.ListenOptions.MaxAllowedBindings = 3;

      playerActions.ListenOptions.OnBindingFound = OnBindingFound;

      return playerActions;
    }

    private static bool OnBindingFound(PlayerAction action, BindingSource binding)
    {
      if (binding != new KeyBindingSource(Key.Escape))
        return true;

      action.StopListeningForBinding();

      return false;
    }
  }
}
