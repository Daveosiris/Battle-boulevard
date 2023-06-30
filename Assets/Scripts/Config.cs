public class Config
{
	public static string version = "v 1.4.11";

	public static Platform platform = Platform.Android;

	public static string PP_Orange_Count = "OC";

	public static int CityTotalLevel = 24;

	public static string PP_CityLevel_Opened = "CityLVLOpened";

	public static string PP_CityLevel_Index = "CityLVL";

	public static string PP_SelectedCharacter_ID = "SelectedCHid";

	public static string PP_Character = "CH";

	public static string PP_CharacterHealth = ".ChH";

	public static string PP_CharacterHealthUpgradeCount = ".ChHUC";

	public static string PP_CharacterPunchPower = ".ChPP.";

	public static string PP_CharacterPunchPowerUpgradeCount = ".ChPPUC";

	public static string PP_CharacterKickPower = ".ChKP.";

	public static string PP_CharacterKickPowerUpgradeCount = ".ChKPUC";

	public static string PP_CharacterWeaponTecnique = ".ChWT";

	public static string PP_CharacterWeaponTecniqueUpgradeCount = ".ChWTUC";

	public static string PP_Music = "Mu";

	public static string PP_Sound = "So";

	public static string PP_DynmJoy = "Dj";

	public static string PP_Haptic = "Ha";

	public static string PP_Removed_Ads = "RADS";

	public static string PP_Diffuculty = "DIFF";

	public static string[] Diffuculties = new string[4]
	{
		"Easy",
		"Normal",
		"Hard",
		"Insane"
	};

	public static string[] LevelDiffucultyPrefix = new string[4]
	{
		string.Empty,
		"n",
		"h",
		"i"
	};

	public static float[] DiffucultyMultiplayers = new float[4]
	{
		1f,
		1.5f,
		2f,
		3f
	};

	public static float EnemyPower = 2f;

	public static float EnemyWeaponTeqnique = 0.25f;

	public static float EnemyLevelHealth = 90f;

	public static float BossPower = 3f;

	public static float BossWeaponTeqnique = 0.5f;

	public static float BossHealth = 25f;

	public static float SPBossPower = 6f;

	public static float SPBossWeaponTeqnique = 1f;

	public static float SPBossHealth = 70f;

	public static float ArcadeLevelFixedEnemyHealth = 2f;

	public static float DropCrateDamage = 0.5f;

	public static float LevelValuesMultiplyer = 0.05f;

	public static float LevelOrangeCount = 20f;

	public static int MaximumSpawnOrangeCount = 5;

	public static int MaximumUpgradeCount = 5;

	public static float MaximumPlayerHealth = 72.5f;

	public static float MaximumPlayerWeaponTecnique = 5f;

	public static float MaximumPlayerPunchPower = 24f;

	public static float MaximumPlayerKickPower = 24f;

	public static float FORCE_MULTIPLYER = 13000f;

	public static string[] MaleNames = new string[13]
	{
		"Anthony",
		"Brian",
		"Chris",
		"Daniel",
		"David",
		"George",
		"James",
		"Jeff",
		"John",
		"Mark",
		"Robert",
		"Steven",
		"Thomas"
	};

	public static string[] FemaleNames = new string[13]
	{
		"Ashley",
		"Britany",
		"Christian",
		"Daniela",
		"Angel",
		"Giselle",
		"Jennifer",
		"Sarah",
		"Sandra",
		"Mery",
		"Rose",
		"Summer",
		"Tiffany"
	};

	public static string[] WeaponNames = new string[8]
	{
		"0",
		"1BaseBallBat",
		"2Wrench",
		"3Crowbar",
		"4Hammer",
		"5Mallet",
		"6Knife",
		"7Gun"
	};
}
