namespace ControlFreak2.Demos
{
	public class MultiDemoManager : GameState
	{
		private const string MAIN_MENU_SCENE_NAME = "CF2-Multi-Demo-Manager";

		public DemoMainState mainMenuState;

		public void EnterMainMenu()
		{
			StartSubState(mainMenuState);
		}

		private void Start()
		{
			OnStartState(null);
		}

		private void Update()
		{
			if (IsRunning())
			{
				OnUpdateState();
			}
		}

		protected override void OnStartState(GameState parentState)
		{
			base.OnStartState(parentState);
			base.gameObject.SetActive(value: true);
			EnterMainMenu();
		}

		protected override void OnExitState()
		{
			base.OnExitState();
			base.gameObject.SetActive(value: false);
		}
	}
}
