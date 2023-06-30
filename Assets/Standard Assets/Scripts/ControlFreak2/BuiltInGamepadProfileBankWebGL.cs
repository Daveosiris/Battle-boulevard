namespace ControlFreak2
{
	public class BuiltInGamepadProfileBankWebGL : BuiltInGamepadProfileBank
	{
		public BuiltInGamepadProfileBankWebGL()
		{
			profiles = new GamepadProfile[3]
			{
				new GamepadProfile("XBOX 360", "xinput", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(3, horzPositiveRight: true, 4, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(5, horzPositiveRight: true, 6, vertPositiveUp: true), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.PlusAxis(8), GamepadProfile.KeySource.PlusAxis(9), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9)),
				new GamepadProfile("PSX", "Twin USB Joystick", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(3, horzPositiveRight: true, 2, vertPositiveUp: false), GamepadProfile.JoystickSource.Dpad(13, 16, 14, 15), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.Key(10), GamepadProfile.KeySource.Key(11)),
				new GamepadProfile("MOGA", "Android Controller", GamepadProfile.ProfileMode.Regex, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(2, horzPositiveRight: true, 3, vertPositiveUp: false), GamepadProfile.JoystickSource.Dpad(10, 13, 11, 12), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(-1), GamepadProfile.KeySource.Key(11), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.PlusAxis(5), GamepadProfile.KeySource.PlusAxis(4), GamepadProfile.KeySource.Key(13), GamepadProfile.KeySource.Key(-1))
			};
		}
	}
}
