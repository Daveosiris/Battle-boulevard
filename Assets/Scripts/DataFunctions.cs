using CodeStage.AntiCheat.ObscuredTypes;

public class DataFunctions
{
	public static void SaveData(ObscuredString Key, ObscuredInt Value)
	{
		ObscuredPrefs.SetInt(Key, Value);
	}

	public static void SaveData(ObscuredString Key, ObscuredFloat Value)
	{
		ObscuredPrefs.SetFloat(Key, Value);
	}

	public static int GetIntData(ObscuredString Key)
	{
		return ObscuredPrefs.GetInt(Key);
	}

	public static float GetFloatData(ObscuredString Key)
	{
		return ObscuredPrefs.GetFloat(Key);
	}

	public static bool HasData(ObscuredString Key)
	{
		return ObscuredPrefs.HasKey(Key);
	}

	public static void DeleteAll()
	{
		ObscuredPrefs.DeleteAll();
	}
}
