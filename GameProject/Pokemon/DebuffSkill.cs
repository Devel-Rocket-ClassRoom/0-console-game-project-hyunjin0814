public class DebuffSill : Skill
{
    private string _debuffStat;
    private int _debuffLevel;

    public DebuffSill(string name, string type, string debuffStat, int debuffLevel) : base(name, type)
    {
        _debuffStat = debuffStat;   
        _debuffLevel = debuffLevel;   
    }

    public void Debuff(Pokemon target)
    {
    }
}
