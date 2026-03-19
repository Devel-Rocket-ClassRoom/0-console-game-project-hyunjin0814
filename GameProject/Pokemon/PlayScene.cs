using System;
using Framework.Engine;

public class PlayScene : Scene
{
    private Wall wall;
    private Player player;
    private Trainer npc;

    public event GameAction BattleRequested;
    public event GameAction PlayAgainRequested;

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
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            BattleRequested?.Invoke();
        }
        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            PlayAgainRequested?.Invoke();
        }
    }
    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);
    }
}