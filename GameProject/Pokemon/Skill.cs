public class Skill
{
    public string Name { get; private set; }
    public PokemonType Type { get; private set; }
    public int PowerRate { get; private set; }
    public int HitRate { get; private set; }
    public string Description { get; private set; }


    public Skill(string name, PokemonType type, int powerRate, int hitRate, string description)
    {
        Name = name;
        Type = type;
        PowerRate = powerRate;
        HitRate = hitRate;
        Description = description;
    }
}