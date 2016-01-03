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

      AssignDefaultBindings();
    }

    private void AssignDefaultBindings()
    {
      this.moveLeft.AddDefaultBinding(Key.A);
      this.moveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
      this.moveLeft.AddDefaultBinding(InputControlType.DPadLeft);

      this.moveRight.AddDefaultBinding(Key.D);
      this.moveRight.AddDefaultBinding(InputControlType.LeftStickRight);
      this.moveRight.AddDefaultBinding(InputControlType.DPadRight);

      this.fartLeft.AddDefaultBinding(InputControlType.RightStickLeft);
      this.fartRight.AddDefaultBinding(InputControlType.RightStickRight);
      this.fartDown.AddDefaultBinding(InputControlType.RightStickDown);
      this.fartUp.AddDefaultBinding(InputControlType.RightStickUp);

      Jump.AddDefaultBinding(Key.Space);
      Jump.AddDefaultBinding(InputControlType.Action1);

      this.ListenOptions.IncludeUnknownControllers = false;
      this.ListenOptions.MaxAllowedBindings = 3;
      this.ListenOptions.OnBindingFound = OnBindingFound;
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
