using InControl;

namespace PachowStudios.BadTummyBunny
{
  public class PlayerInput : PlayerActionSet
  {
    private PlayerAction MoveLeft { get; }
    private PlayerAction MoveRight { get; }

    private PlayerAction FartLeft { get; }
    private PlayerAction FartRight { get; }
    private PlayerAction FartDown { get; }
    private PlayerAction FartUp { get; }

    public PlayerOneAxisAction Move { get; }
    public PlayerTwoAxisAction Fart { get; }

    public PlayerAction Jump { get; }

    public PlayerInput()
    {
      MoveLeft = CreatePlayerAction("Move Left");
      MoveRight = CreatePlayerAction("Move Right");

      FartLeft = CreatePlayerAction("Fart Aim Left");
      FartRight = CreatePlayerAction("Fart Aim Right");
      FartDown = CreatePlayerAction("Fart Aim Down");
      FartUp = CreatePlayerAction("Fart Aim Up");

      Move = CreateOneAxisPlayerAction(MoveLeft, MoveRight);
      Fart = CreateTwoAxisPlayerAction(FartLeft, FartRight, FartDown, FartUp);

      Jump = CreatePlayerAction("Jump");

      AssignDefaultBindings();
    }

    private void AssignDefaultBindings()
    {
      MoveLeft.AddDefaultBinding(Key.A);
      MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
      MoveLeft.AddDefaultBinding(InputControlType.DPadLeft);

      MoveRight.AddDefaultBinding(Key.D);
      MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
      MoveRight.AddDefaultBinding(InputControlType.DPadRight);

      FartLeft.AddDefaultBinding(InputControlType.RightStickLeft);
      FartRight.AddDefaultBinding(InputControlType.RightStickRight);
      FartDown.AddDefaultBinding(InputControlType.RightStickDown);
      FartUp.AddDefaultBinding(InputControlType.RightStickUp);

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
