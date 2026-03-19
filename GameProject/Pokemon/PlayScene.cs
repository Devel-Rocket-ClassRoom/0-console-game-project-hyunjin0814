using System;
using Framework.Engine;

public class PlayScene : Scene
{
    private Wall wall;

    public override void Load()
    {
        wall = new Wall(this);
        AddGameObject(wall);
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        UpdateGameObjects(deltaTime);
    }
    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);
    }
}