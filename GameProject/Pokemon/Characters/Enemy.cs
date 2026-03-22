using Framework.Engine;

public class Enemy : Trainer
{
    public bool IsDefeated { get; set; } = false;

    public Enemy(Scene scene, int startX, int startY, string name) : base(scene, startX, startY, name)
    {
    }

    public void GetPokemons(Pokemon[] p)
    {
        pokemons[0] = p[0];
        pokemons[1] = p[1];
        pokemons[2] = p[2];
    }
}