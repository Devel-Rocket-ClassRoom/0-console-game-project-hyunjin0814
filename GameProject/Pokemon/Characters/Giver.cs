using Framework.Engine;

public class Giver : Character
{
    public bool IsActive { get; set; } = true;

    public Giver(Scene scene, int startX, int startY, string name) : base(scene, startX, startY, name)
    {
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (!IsActive) return;

        // 트레이너(Red 'T')와 구분되도록 초록색 'G'나 'P(Professor)'로 표시해봅시다.
        buffer.SetCell(Position.X, Position.Y, 'G', ConsoleColor.Green);
    }
}