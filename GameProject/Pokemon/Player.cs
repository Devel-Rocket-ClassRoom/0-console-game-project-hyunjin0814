using Framework.Engine;
using System;
using System.Xml.Linq;

public class Player : GameObject
{
    private (int X, int Y) _position;

    public (int X, int Y) Position => _position;

    public Player(Scene scene, int startX, int startY) : base(scene)
    {
        Name = "Player";

        _position.X = startX;
        _position.Y = startY;
    }

    private void Move()
    {
        if (Input.IsKey(ConsoleKey.UpArrow))
        {
            _position.Y = _position.Y - 1;
        }
        else if (Input.IsKey(ConsoleKey.DownArrow))
        {
            _position.Y = _position.Y + 1;
        }
        else if (Input.IsKey(ConsoleKey.LeftArrow))
        {
            _position.X = _position.X - 1;
        }
        else if (Input.IsKey(ConsoleKey.RightArrow))
        {
            _position.X = _position.X + 1;
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell(Position.X, Position.Y, '@', ConsoleColor.Green);
    }

    public override void Update(float deltaTime)
    {
        Move();
    }
}