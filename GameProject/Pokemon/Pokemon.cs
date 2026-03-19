public class Pokemon : IAttacker, IDefender
{
    public string Name { get; private set; }
    public string Type { get; private set; }
    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }
    public int AttackPower { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public bool IsDead { get; private set; }

    public Pokemon(string name, string type, int hp, int attack, int defense, int speed)
    {
        Name = name;
        Type = type;
        MaxHp = hp;
        CurrentHp = hp;
        AttackPower = attack;
        Defense = defense;
        Speed = speed;
    }

    public void GetBuff(string statName, int buffLevel)
    {
        switch (statName)
        {
            case "공격":
                AttackPower += AttackPower * buffLevel;
                break;
            case "방어":
                Defense += Defense * buffLevel;
                break;
            case "속도":
                Speed += Speed * buffLevel;
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHp = damage - Defense;
    }

    void IAttacker.Attack(IDefender target)
    {

    }
}