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
        // 만약 저장된 정보가 있으면 불러오고 개체를 새로 생성하지 않는 코드 작성 필요
        if (DataManager.hasData)
        {
            player = DataManager.LoadData();
            player.ExitBattle();
        }
        else
        {
            player = new Player(this, 29, 14, "Player");
            Pokemon pokemon1 = new Pokemon("꼬부기", PokemonType.Water, 20, 3, 5, 2);
            pokemon1.GetSkills(SkillChart.waters[0], SkillChart.waters[1], SkillChart.normals[0], SkillChart.normals[1]);
            player.GetPokemon0(pokemon1);
            Pokemon pokemon2 = new Pokemon("이상해씨", PokemonType.Grass, 20, 3, 3, 2);
            pokemon2.GetSkills(SkillChart.grasses[0], SkillChart.grasses[1], SkillChart.normals[0], SkillChart.normals[1]);
            player.GetPokemon1(pokemon2);
        }
        AddGameObject(player);

        wall = new Wall(this);
        AddGameObject(wall);

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

        if (npc.IsInBounds(player.Position.X, player.Position.Y))
        {
            player.PositionSave();

            // 임시로 작성된 배틀씬 전환 테스트 코드
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                DataManager.SaveData(player);
                Thread.Sleep(1000);
                BattleRequested?.Invoke();
            }
        }
        else if (npc.Position == player.Position)
        {
            player.PositionLoad();
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