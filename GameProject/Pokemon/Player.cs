using System;
using Framework.Engine;

public class Player : GameObject
{
    private (int X, int Y) _position;

    public Player(Scene scene, int startX, int startY) : base(scene)
    {
        Name = "Player";

        _position.X = startX;
        _position.Y = startY;
    }
    public override void Draw(ScreenBuffer buffer)
    {
        throw new NotImplementedException();
    }

    public override void Update(float deltaTime)
    {
        throw new NotImplementedException();
    }
}