static class GameManger
{
    public static Skill[] normal = 
    {
        new Skill("몸통박치기", "노말"),
        new Skill("전광석화", "노말"),
        new Skill("막치기", "노말"),
    };

    public static Skill[] water = 
    {
        new Skill("물대포", "물"),
        new Skill("아쿠아제트", "물"),
        new Skill("물의파동", "물"),
    };

    public static Skill[] fire = 
    {
        new Skill("화염방사", "불"),
        new Skill("불꽃엄니", "불"),
        new Skill("불꽃펀치", "불")
    };

    public static Skill[] grass = 
    {
        new Skill("덩굴채찍", "풀"),
        new Skill("매지컬리프", "풀"),
        new Skill("잎날가르기", "풀")
    };

    public static void Battle(Pokemon pokemon1, Pokemon pokemon2)
    {
        while (pokemon1.CurrentHP != 0 || pokemon2.CurrentHP != 0)
        {

        }
    }
}