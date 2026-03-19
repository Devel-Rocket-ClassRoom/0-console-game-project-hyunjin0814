public class AttackSkill : Skill
{
    private int power_rate;
    private int hit_rate;

    public AttackSkill(string name, string type, int power_rate, int hit_rate) : base(name, type)
    {
        this.power_rate = power_rate;
        this.hit_rate = hit_rate;
    }
}