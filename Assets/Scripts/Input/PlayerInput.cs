using InControl;

namespace PachowStudios.BadTummyBunny
{
  public class PlayerInput : PlayerActionSet
  {
    private static readonly KeyBindingSource CancelSetBindingKey = new KeyBindingSource(Key.Escape);

    private PlayerAction MoveLeft { get; }
    private PlayerAction MoveRight { get; }

    private PlayerAction FartLeft { get; }
    private PlayerAction FartRight { get; }
    private PlayerAction FartDown { get; }
    private PlayerAction FartUp { get; }

    public PlayerOneAxisAction Move { get; }
    public PlayerTwoAxisAction Fart { get; }

    public PlayerAction Jump { get; }
    public PlayerAction SecondaryFart { get; }

    public PlayerInput()
    {
      MoveLeft = CreatePlayerAction(nameof(MoveLeft));
      MoveRight = CreatePlayerAction(nameof(MoveRight));

      FartLeft = CreatePlayerAction(nameof(FartLeft));
      FartRight = CreatePlayerAction(nameof(FartRight));
      FartDown = CreatePlayerAction(nameof(FartDown));
      FartUp = CreatePlayerAction(nameof(FartUp));

      Move = CreateOneAxisPlayerAction(MoveLeft, MoveRight);
      Fart = CreateTwoAxisPlayerAction(FartLeft, FartRight, FartDown, FartUp);

      Jump = CreatePlayerAction(nameof(Jump));
      SecondaryFart = CreatePlayerAction(nameof(SecondaryFart));

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

      SecondaryFart.AddDefaultBinding(Key.F);
      SecondaryFart.AddDefaultBinding(InputControlType.Action2);

      this.ListenOptions.IncludeUnknownControllers = false;
      this.ListenOptions.MaxAllowedBindings = 3;
      this.ListenOptions.OnBindingFound = OnBindingFound;
    }

    private static bool OnBindingFound(PlayerAction action, BindingSource binding)
    {
      if (binding != CancelSetBindingKey)
        return true;

      action.StopListeningForBinding();

      return false;
    }
  }
}
