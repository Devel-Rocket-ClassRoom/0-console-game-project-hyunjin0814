using System;
using Framework.Engine;

public class PlayScene : Scene
{
    private Wall wall;
    private Player player;
    private Trainer npc;

    public override void Load()
    {
        wall = new Wall(this);
        AddGameObject(wall);

        player = new Player(this, 29, 14, "Player");
        AddGameObject(player);

        npc = new Trainer(this, 29, 4, "NPC1");
        AddGameObject(npc);
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        UpdateGameObjects(deltaTime);
        if (player.Position.X + 1 == npc.Position.X)
        {
            
        }
    }
    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            buffer.Clear();
        }
    }
}