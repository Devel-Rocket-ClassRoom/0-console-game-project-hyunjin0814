public static class DataManager
{
    public static Player playerData;
    public static bool hasData => playerData != null;

    public static void SaveData(Player player)
    {
        playerData = player;
    }

    public static Player LoadData()
    {
        return playerData;
    }
}