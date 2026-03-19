using System;
using Framework.Engine;

class BattleScene : Scene
{
    private Player player;
    private Trainer enemy;
    private string _log;

    public event GameAction ReturnRequested;

    public override void Load()
    {
        player = new Player(this, 29, 14, "Player");
        AddGameObject(player);
        player.isBattle = true;
        player.GetPokemon(new Pokemon("꼬부기", "물", 20, 3, 5, 2));

        enemy = new Trainer(this, 29, 4, "NPC1");
        AddGameObject(enemy);
        enemy.isBattle = true;
        enemy.GetPokemon(new Pokemon("파이리", "불", 20, 4, 3, 3));
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            ReturnRequested?.Invoke();
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        buffer.WriteTextCentered(25, "전투 로그 메시지", ConsoleColor.White);
    }
}