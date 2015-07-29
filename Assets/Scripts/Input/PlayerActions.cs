using InControl;

public class PlayerActions : PlayerActionSet
{
	private PlayerAction MoveLeft;
	private PlayerAction MoveRight;

	private PlayerAction FartLeft;
	private PlayerAction FartRight;
	private PlayerAction FartDown;
	private PlayerAction FartUp;

	public PlayerOneAxisAction Move;
	public PlayerTwoAxisAction Fart;

	public PlayerAction Jump;

	public PlayerActions()
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
	}

	public static PlayerActions CreateWithDefaultBindings()
	{
		var playerActions = new PlayerActions();

		playerActions.MoveLeft.AddDefaultBinding(Key.A);
		playerActions.MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
		playerActions.MoveLeft.AddDefaultBinding(InputControlType.DPadLeft);

		playerActions.MoveRight.AddDefaultBinding(Key.D);
		playerActions.MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
		playerActions.MoveRight.AddDefaultBinding(InputControlType.DPadRight);

		playerActions.FartLeft.AddDefaultBinding(InputControlType.RightStickLeft);

		playerActions.FartRight.AddDefaultBinding(InputControlType.RightStickRight);

		playerActions.FartDown.AddDefaultBinding(InputControlType.RightStickDown);

		playerActions.FartUp.AddDefaultBinding(InputControlType.RightStickUp);

		playerActions.Jump.AddDefaultBinding(Key.Space);
		playerActions.Jump.AddDefaultBinding(InputControlType.Action1);

		playerActions.ListenOptions.IncludeUnknownControllers = false;
		playerActions.ListenOptions.MaxAllowedBindings = 3;

		playerActions.ListenOptions.OnBindingFound = OnBindingFound;

		return playerActions;
	}

	private static bool OnBindingFound(PlayerAction action, BindingSource binding)
	{
		if (binding == new KeyBindingSource(Key.Escape))
		{
			action.StopListeningForBinding();
			return false;
		}

		return true;
	}
}
