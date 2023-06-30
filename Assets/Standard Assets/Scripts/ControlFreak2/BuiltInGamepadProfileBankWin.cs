namespace ControlFreak2
{
	public class BuiltInGamepadProfileBankWin : BuiltInGamepadProfileBank
	{
		public BuiltInGamepadProfileBankWin()
		{
			profiles = new GamepadProfile[6]
			{
				new GamepadProfile("nVidia Shield", "nVidia", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(2, horzPositiveRight: true, 3, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(4, horzPositiveRight: true, 5, vertPositiveUp: true), GamepadProfile.KeySource.Key(9), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(11), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.PlusAxis(-1), GamepadProfile.KeySource.MinusAxis(-1), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(2)),
				new GamepadProfile("XBOX 360", "Controller (XBOX 360 For Windows)", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(3, horzPositiveRight: true, 4, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(5, horzPositiveRight: true, 6, vertPositiveUp: true), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.PlusAxis(8), GamepadProfile.KeySource.PlusAxis(9), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9)),
				new GamepadProfile("XBOX 360", "XBOX 360", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(3, horzPositiveRight: true, 4, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(5, horzPositiveRight: true, 6, vertPositiveUp: true), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.PlusAxis(8), GamepadProfile.KeySource.PlusAxis(9), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9)),
				new GamepadProfile("MOGA", "Android Controller Gen-2(ACC)", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(2, horzPositiveRight: true, 4, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(4, horzPositiveRight: true, 5, vertPositiveUp: true), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.Key(-1), GamepadProfile.KeySource.Key(-1), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9)),
				new GamepadProfile("MOGA", "Android Controller", GamepadProfile.ProfileMode.Regex, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(2, horzPositiveRight: true, 4, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(4, horzPositiveRight: true, 5, vertPositiveUp: true), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.Empty(), GamepadProfile.KeySource.Empty(), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9)),
				new GamepadProfile("PSX", "Twin USB Joystick", GamepadProfile.ProfileMode.Normal, null, null, GamepadProfile.JoystickSource.Axes(0, horzPositiveRight: true, 1, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(3, horzPositiveRight: true, 2, vertPositiveUp: false), GamepadProfile.JoystickSource.Axes(4, horzPositiveRight: true, 5, vertPositiveUp: true), GamepadProfile.KeySource.Key(2), GamepadProfile.KeySource.Key(1), GamepadProfile.KeySource.Key(3), GamepadProfile.KeySource.Key(0), GamepadProfile.KeySource.Key(8), GamepadProfile.KeySource.Key(9), GamepadProfile.KeySource.Key(6), GamepadProfile.KeySource.Key(7), GamepadProfile.KeySource.Key(4), GamepadProfile.KeySource.Key(5), GamepadProfile.KeySource.Key(10), GamepadProfile.KeySource.Key(11))
			};
		}
	}
}
