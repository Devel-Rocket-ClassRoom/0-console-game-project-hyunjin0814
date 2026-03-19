using System;
using Framework.Engine;

public class Trainer : GameObject
{
    protected Pokemon[] pokemons = new Pokemon[3];
    
    protected (int X, int Y) _position;
    public bool isBattle; // 테스트용 public, protected로 수정 필요

    protected string _name;
    public (int X, int Y) Position => _position;

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
            // 처음에는 첫 번째 포켓몬을 출력하도록 수정
            buffer.WriteText(45, 2, $"[{pokemons[0].Name}] {pokemons[0].CurrentHP}/{pokemons[0].MaxHp}", ConsoleColor.Red);
            buffer.WriteText(45, 3, " /\\_/\\");
            buffer.WriteText(45, 4, "( o.o )");
            buffer.WriteText(45, 5, " > ^ <");
        }
        else
        {
            buffer.SetCell(Position.X, Position.Y, 'T', ConsoleColor.Red);
        }
    }

    // 테스트용 코드
    public void GetPokemon(Pokemon pokemon)
    {
        pokemons[0] = pokemon;
    }
}