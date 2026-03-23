using System;
using Framework.Engine;

public class TitleScene : Scene
{
    public event GameAction StartRequested;

    public override void Load()
    {
    }

    public override void Unload()
    {
    }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            StartRequested?.Invoke();
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        buffer.WriteTextCentered(15, "[Tower Of Pokemon]", ConsoleColor.Blue);
        buffer.WriteTextCentered(18, "Enter 키를 눌러서 시작하세요.", ConsoleColor.DarkCyan);
        buffer.WriteTextCentered(20, "방향키로 움직이세요.", ConsoleColor.DarkCyan);
    }
}