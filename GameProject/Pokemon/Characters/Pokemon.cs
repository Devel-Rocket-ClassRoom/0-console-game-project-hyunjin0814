using System;
using System.Xml.Linq;

public class Pokemon
{
    public string Name { get; private set; }
    public PokemonType Type { get; private set; }
    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }
    public int Level { get; private set; } = 1;
    public int AttackPower { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public bool IsDead { get; private set; }
    public Skill[] skills { get; private set; }
    private static Random _random = new Random();

    public Pokemon(string name, PokemonType type, int hp, int level, int attack, int defense, int speed)
    {
        Name = name;
        Type = type;
        MaxHp = hp;
        CurrentHp = hp;
        Level = level;
        AttackPower = attack;
        Defense = defense;
        Speed = speed;
        IsDead = false;
        skills = new Skill[4];
    }

    public Pokemon(string name, PokemonType type, int hp, int level, int attack, int defense, int speed, Skill skill1, Skill skill2, Skill skill3, Skill skill4) : this(name, type, hp, level, attack, defense, speed)
    {
        skills[0] = skill1;
        skills[1] = skill2;
        skills[2] = skill3;
        skills[3] = skill4;
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

    // 임시 레벨업 로직
    public void LevelUp(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Level++;
            MaxHp += 5;        
            AttackPower += 1;  
            Defense += 1;      
            Speed += 1;        
        }
    }

    public int AttackTo(Skill skill, Pokemon target, out float multiplier, out bool isHit)
    {
        int roll = _random.Next(0, 101);
        if (roll > skill.HitRate)
        {
            multiplier = 1.0f; // 빗나갔으므로 배수는 기본값
            isHit = false;     // 명중 실패
            return 0;          // 데미지 0 반환
        }
        isHit = true;
        multiplier = TypeChart.GetMultiplier(skill.Type, target.Type);
        int finalDamage = (int)((skill.PowerRate + this.AttackPower) * multiplier);
        return finalDamage;
    }

    public void TakeDamage(int damage)
    {
        if (damage == 0)
        {
            return;
        }
        else if (damage - Defense < 1)
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

    public void Heal()
    {
        CurrentHp = MaxHp;
        IsDead = false;
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