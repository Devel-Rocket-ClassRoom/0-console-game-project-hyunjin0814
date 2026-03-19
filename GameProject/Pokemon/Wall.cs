using System;
using Framework.Engine;

public class Wall : GameObject
{
    public const int Left = 1;
    public const int Top = 1;
    public const int Right = 58;
    public const int Bottom = 28;

    public Wall(Scene scene) : base(scene)
    {
        Name = "Wall";
    }

    public override void Update(float deltaTime)
    {
    }
    public override void Draw(ScreenBuffer buffer)
    {
        buffer.DrawBox(Left - 1, Top - 1, Right - Left + 3, Bottom - Top + 3, ConsoleColor.White);
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= Left && x <= Right && y >= Top && y <= Bottom;
    }

    public bool IsInBounds((int X, int Y) position)
    {
        return IsInBounds(position.X, position.Y);
    }
}
