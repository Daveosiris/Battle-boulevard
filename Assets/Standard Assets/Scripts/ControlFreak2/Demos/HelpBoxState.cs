namespace ControlFreak2.Demos
{
	public class HelpBoxState : GameState
	{
		protected DemoMainState parentDemoState;

		protected override void OnStartState(GameState parentState)
		{
			base.OnStartState(parentState);
			base.gameObject.SetActive(value: true);
		}

		protected override void OnExitState()
		{
			base.OnExitState();
			base.gameObject.SetActive(value: false);
		}

		public void ShowHelpBox(DemoMainState parentDemoState)
		{
			this.parentDemoState = parentDemoState;
			this.parentDemoState.StartSubState(this);
		}

		public void ExitToMainMenu()
		{
			if (parentDemoState != null)
			{
				parentDemoState.ExitToMainMenu();
			}
		}
	}
}
