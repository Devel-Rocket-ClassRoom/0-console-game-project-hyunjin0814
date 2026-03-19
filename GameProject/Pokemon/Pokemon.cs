public class Pokemon
{
    public string Name { get; private set; }
    public string Type { get; private set; }
    public int CurrentHP { get; private set; }
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }

    public Pokemon(string name, string type, int hp, int attack, int defense, int speed)
    {
        Name = name;
        Type = type;
        MaxHp = hp;
        CurrentHP = hp;
        Attack = attack;
        Defense = defense;
        Speed = speed;
    }

    public void GetBuff(string statName, int buffLevel)
    {
        switch (statName)
        {
            case "공격":
                Attack += Attack * buffLevel;
                break;
            case "방어":
                Defense += Defense * buffLevel;
                break;
            case "속도":
                Speed += Speed * buffLevel;
                break;
        }
    }
}