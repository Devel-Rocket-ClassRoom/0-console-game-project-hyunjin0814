using System;
using Framework.Engine;

public class Trainer : Character
{
    protected Pokemon[] pokemons = new Pokemon[3];
    
    public bool isBattle; // 테스트용 public, protected로 수정 필요
    public Pokemon[] Pokemons => pokemons;

    public Trainer(Scene scene, int startX, int startY, string name) : base(scene, startX, startY, name)
    {
        isBattle = false;
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (isBattle)
        {
            return;
        }
        buffer.SetCell(Position.X, Position.Y, 'T', ConsoleColor.Red);
    }

    public void StartBattle()
    {
        isBattle = true;
    }

    public void ExitBattle()
    {
        isBattle = false;
    }

    // 테스트용 코드
    public void GetPokemon0(Pokemon pokemon)
    {
        pokemons[0] = pokemon;
    }
    public void GetPokemon1(Pokemon pokemon)
    {
        pokemons[1] = pokemon;
    }
    public void GetPokemon2(Pokemon pokemon)
    {
        pokemons[2] = pokemon;
    }
}