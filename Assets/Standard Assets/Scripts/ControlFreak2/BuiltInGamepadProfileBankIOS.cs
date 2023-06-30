namespace ControlFreak2
{
	public class BuiltInGamepadProfileBankIOS : BuiltInGamepadProfileBank
	{
		public BuiltInGamepadProfileBankIOS()
		{
			genericProfile = new GamepadProfile("MFi Controller", "MFi Controller", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(2, horzPositiveRight: true, 3, vertPositiveUp: false), GamepadProfile.JoystickSource.Dpad(4, 5, 6, 7), GamepadProfile.KeySource.Key(14), GamepadProfile.KeySource.Key(13), GamepadProfile.KeySource.Key(15), GamepadProfile.KeySource.Key(12), GamepadProfile.KeySource.Empty(), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.KeyAndPlusAxis(8, 8), GamepadProfile.KeySource.KeyAndPlusAxis(9, 9), GamepadProfile.KeySource.KeyAndPlusAxis(10, 10), GamepadProfile.KeySource.KeyAndPlusAxis(11, 11), GamepadProfile.KeySource.Key(-1), GamepadProfile.KeySource.Key(-1));
			profiles = new GamepadProfile[1]
			{
				new GamepadProfile("Startus XL", "Startus XL", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(2, horzPositiveRight: true, 3, vertPositiveUp: false), GamepadProfile.JoystickSource.Dpad(4, 5, 6, 7), GamepadProfile.KeySource.Key(14), GamepadProfile.KeySource.Key(13), GamepadProfile.KeySource.Key(15), GamepadProfile.KeySource.Key(12), GamepadProfile.KeySource.Empty(), GamepadProfile.KeySource.Key(-1), GamepadProfile.KeySource.KeyAndPlusAxis(8, 8), GamepadProfile.KeySource.KeyAndPlusAxis(9, 9), GamepadProfile.KeySource.Key(10), GamepadProfile.KeySource.Key(11), GamepadProfile.KeySource.Key(-1), GamepadProfile.KeySource.Key(-1))
			};
		}
	}
}
