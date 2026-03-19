using System;
using Framework.Engine;

public class Trainer : GameObject
{
    protected Pokemon[] pokemons = new Pokemon[3];
    protected (int X, int Y) _position;

    protected string _name;
    public (int X, int Y) Position => _position;

    public Trainer(Scene scene, int startX, int startY, string name) : base(scene)
    {
        _position.X = startX;
        _position.Y = startY;
        _name = name;
    }

    public override void Update(float deltaTime)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell(Position.X, Position.Y, 'T', ConsoleColor.Red);
    }
}