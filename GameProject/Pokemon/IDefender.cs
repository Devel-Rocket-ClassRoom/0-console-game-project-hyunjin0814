interface IDefender
{

    int CurrentHp { get; }
    int MaxHp { get; }
    bool IsDead { get; }
    void TakeDamage(int damage);
}