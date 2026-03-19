static class GameManger
{
    private static Random _rand = new Random();

    public static Skill GetEnemyRandomSkill(Pokemon enemy)
    {
        int index = _rand.Next(0, 4);
        return enemy.skills[index];
    }
}