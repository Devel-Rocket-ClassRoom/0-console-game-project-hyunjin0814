public static class DataManager
{
    public static Player playerData;
    public static Enemy currentEnemyData;
    public static List<Enemy> npcList;
    public static bool isTutorialDone = false;
    public static int currentStage = 0;

    public static bool hasData => playerData != null;

    public static void SaveData(Player player) => playerData = player;
    public static Player LoadData() => playerData;

    public static void SaveEnemyData(Enemy enemy) => currentEnemyData = enemy;
    public static Enemy LoadEnemyData() => currentEnemyData;

    public static void SaveNPCList(List<Enemy> npcs) => npcList = npcs;
}