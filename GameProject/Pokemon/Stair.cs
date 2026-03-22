using Framework.Engine;

public class Stair : GameObject
{
    public Stair(Scene scene) : base(scene)
    {
    }

    public bool IsActive { get; set; } = false;

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsActive) return;
        buffer.SetCell(58, 1, 'S', ConsoleColor.Cyan);
    }

    public override void Update(float deltaTime)
    {
    }
}