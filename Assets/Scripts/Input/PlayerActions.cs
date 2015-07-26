using InControl;

public class PlayerActions : PlayerActionSet
{
	private PlayerAction MoveLeft;
	private PlayerAction MoveRight;

	private PlayerAction FartAimLeft;
	private PlayerAction FartAimRight;
	private PlayerAction FartAimDown;
	private PlayerAction FartAimUp;

	public PlayerOneAxisAction Move;
	public PlayerTwoAxisAction FartAim;

	public PlayerAction Jump;

	public PlayerActions()
	{
		MoveLeft = CreatePlayerAction("Move Left");
		MoveRight = CreatePlayerAction("Move Right");

		FartAimLeft = CreatePlayerAction("Fart Aim Left");
		FartAimRight = CreatePlayerAction("Fart Aim Right");
		FartAimDown = CreatePlayerAction("Fart Aim Down");
		FartAimUp = CreatePlayerAction("Fart Aim Up");

		Move = CreateOneAxisPlayerAction(MoveLeft, MoveRight);
		FartAim = CreateTwoAxisPlayerAction(FartAimLeft, FartAimRight, FartAimDown, FartAimUp);

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

		playerActions.FartAimLeft.AddDefaultBinding(InputControlType.RightStickLeft);

		playerActions.FartAimRight.AddDefaultBinding(InputControlType.RightStickRight);

		playerActions.FartAimDown.AddDefaultBinding(InputControlType.RightStickDown);

		playerActions.FartAimUp.AddDefaultBinding(InputControlType.RightStickUp);

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
