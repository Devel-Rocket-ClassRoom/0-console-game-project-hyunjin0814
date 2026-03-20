using System;
using Framework.Engine;

public class Trainer : GameObject
{
    protected Pokemon[] pokemons = new Pokemon[3];
    
    protected (int X, int Y) _position;
    public bool isBattle; // 테스트용 public, protected로 수정 필요

    protected string _name;
    public (int X, int Y) Position => _position;
    public Pokemon[] Pokemons => pokemons;

    public Trainer(Scene scene, int startX, int startY, string name) : base(scene)
    {
        _position.X = startX;
        _position.Y = startY;
        _name = name;
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
    public void GetPokemon(Pokemon pokemon)
    {
        pokemons[0] = pokemon;
    }
}