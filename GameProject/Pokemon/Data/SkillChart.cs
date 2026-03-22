public static class SkillChart
{
    public static Skill[] normals =
    {
        new Skill("할퀴기", PokemonType.Normal, 1, 100, "손톱으로 상대를 할퀴어서 공격한다"),
        new Skill("몸통박치기", PokemonType.Normal, 2, 90, "몸 전체를 부딪쳐가며 공격한다."),
        new Skill("분노의앞니", PokemonType.Normal, 4, 80, "앞니로 강하게 물어서 공격한다."),
        new Skill("막치기", PokemonType.Normal, 2, 100, "긴 꼬리나 손 등을 사용하여 상대를 공격한다.")
    };

    public static Skill[] waters =
    {
        new Skill("물대포", PokemonType.Water, 1, 100, "물을 상대에게 발사하여 공격한다."),
        new Skill("거품광선", PokemonType.Water, 2, 90, "거품을 상대에게 발사하여 공격한다."),
        new Skill("물의파동", PokemonType.Water, 4, 80, "물의 진동을 상대에게 가하여 공격한다.")
    };

    public static Skill[] fires =
    {
        new Skill("니트로차지", PokemonType.FIre, 1, 100, "불꽃을 둘러 상대를 공격한다."),
        new Skill("화염방사", PokemonType.FIre, 2, 90, "불꽃을 상대에게 발사해서 공격한다"),
        new Skill("불꽃엄니", PokemonType.FIre, 4, 80, "불꽃을 두른 이빨로 문다.")
    };

    public static Skill[] grasses =
    {
        new Skill("덩굴채찍", PokemonType.Grass, 1, 100, "덩굴로 상대를 힘껏 쳐서 공격한다."),
        new Skill("잎날가르기", PokemonType.Grass, 2, 90, "잎사귀를 날려 상대를 베어 공격한다."),
        new Skill("매지컬리프", PokemonType.Grass, 4, 80, "상대를 추적하는 잎사귀를 흩뿌린다.")
    };
}