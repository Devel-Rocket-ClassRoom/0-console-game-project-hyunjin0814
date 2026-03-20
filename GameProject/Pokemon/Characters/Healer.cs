using Framework.Engine;

public class Healer : Character
{
    public Healer(Scene scene, int startX, int startY, string name) : base(scene, startX, startY, name)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.SetCell(Position.X, Position.Y, 'H', ConsoleColor.Green);
    }
}