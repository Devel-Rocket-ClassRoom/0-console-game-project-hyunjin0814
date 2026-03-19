public class Pokemon
{
    private string _name;
    private string _type;

    private int _hp;
    private int _attack;
    private int _defense;
    private int _speed;

    public Pokemon(string name, string type, int hp, int attack, int defense, int speed)
    {
        _name = name;
        _type = type;
        _hp = hp;
        _attack = attack;
        _defense = defense;
        _speed = speed;
    }

    public void GetBuff(string statName, int buffLevel)
    {
        switch (statName)
        {
            case "공격":
                _attack += _attack * buffLevel;
                break;
            case "방어":
                _defense += _defense * buffLevel;
                break;
            case "속도":
                _speed += _speed * buffLevel;
                break;
        }
    }
}