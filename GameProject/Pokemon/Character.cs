using System;
using Framework.Engine;

public class Character : GameObject
{
    protected (int X, int Y) _position;

    protected string _name;
    public (int X, int Y) Position => _position;

    public Character(Scene scene, int startX, int startY, string name) : base(scene)
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
        buffer.SetCell(Position.X, Position.Y, '+', ConsoleColor.Blue);
    }
    public bool IsInBounds(int x, int y)
    {
        return (x == Position.X && y == Position.Y - 1) || (x == Position.X && y == Position.Y + 1)
            || (x == Position.X - 1 && y == Position.Y) || (x == Position.X + 1 && y == Position.Y);
    }
}