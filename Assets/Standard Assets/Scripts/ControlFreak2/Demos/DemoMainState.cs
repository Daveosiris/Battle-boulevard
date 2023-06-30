using UnityEngine;

namespace ControlFreak2.Demos
{
	public class DemoMainState : GameState
	{
		public MultiDemoManager multiDemoManager;

		public HelpBoxState helpBox;

		public KeyCode helpKey = KeyCode.Escape;

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

		protected override void OnUpdateState()
		{
			if (helpBox != null && !helpBox.IsRunning() && helpKey != 0 && CF2Input.GetKeyDown(helpKey))
			{
				ShowHelpBox();
			}
			base.OnUpdateState();
		}

		public void ExitToMainMenu()
		{
			if (multiDemoManager != null)
			{
				multiDemoManager.EnterMainMenu();
			}
			else
			{
				Application.Quit();
			}
		}

		public void ShowHelpBox()
		{
			if (helpBox != null)
			{
				helpBox.ShowHelpBox(this);
			}
		}
	}
}
