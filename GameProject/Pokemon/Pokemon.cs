public class Pokemon
{
    public string Name { get; private set; }
    public PokemonType Type { get; private set; }
    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }
    public int AttackPower { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public bool IsDead { get; private set; }
    public Skill[] skills { get; private set; }

    public Pokemon(string name, PokemonType type, int hp, int attack, int defense, int speed)
    {
        Name = name;
        Type = type;
        MaxHp = hp;
        CurrentHp = hp;
        AttackPower = attack;
        Defense = defense;
        Speed = speed;
        IsDead = false;
        skills = new Skill[4];
    }

    // 추후 구현
    //public void GetBuff(string statName, int buffLevel)
    //{
    //    switch (statName)
    //    {
    //        case "공격":
    //            AttackPower += AttackPower * buffLevel;
    //            break;
    //        case "방어":
    //            Defense += Defense * buffLevel;
    //            break;
    //        case "속도":
    //            Speed += Speed * buffLevel;
    //            break;
    //    }
    //}

    public int AttackTo(Skill skill, Pokemon target)
    {
        float multiplier = TypeChart.GetMultiplier(skill.Type, target.Type);
        int finalDamage = (int)((skill.PowerRate + this.AttackPower) * multiplier);
        return finalDamage;
    }

    public void TakeDamage(int damage)
    {
        if (damage - Defense < 1)
        {
            CurrentHp -= 1;
        }
        else
        {
            CurrentHp -= (damage - Defense);
        }
            
        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            IsDead = true;
        }
    }

    // 테스트용 스킬 할당
    public void GetSkills(Skill skill1, Skill skill2, Skill skill3, Skill skill4)
    {
        skills[0] = skill1;
        skills[1] = skill2;
        skills[2] = skill3;
        skills[3] = skill4;
    }
}