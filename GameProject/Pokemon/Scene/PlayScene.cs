using System;
using Framework.Engine;

public class PlayScene : Scene
{
    private Wall wall;
    private Player player;
    private Trainer npc;
    private Healer healer;
    private string _currentLog = string.Empty;
    //private PlayerState _playerState;

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
            Pokemon pokemon3 = new Pokemon("파이리1", PokemonType.FIre, 20, 10, 3, 3);
            pokemon3.GetSkills(SkillChart.fires[2], SkillChart.fires[1], SkillChart.normals[2], SkillChart.normals[1]);
            player.GetPokemon2(pokemon3);
        }
        AddGameObject(player);

        wall = new Wall(this);
        AddGameObject(wall);

        npc = new Trainer(this, 29, 4, "NPC1");
        AddGameObject(npc);

        healer = new Healer(this, 1, 11, "Healer");
        AddGameObject(healer);
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        UpdateGameObjects(deltaTime);

        // 플레이어의 경계에 있는 개체의 정보를 저장하고 그 개체에 맞춰서 하는 행동을 다르게 하면 좋을듯 (다운캐스팅, 패턴매칭 활용)
        if (player.IsInBounds(npc.Position.X, npc.Position.Y))
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

        if (player.IsInBounds(healer.Position.X, healer.Position.Y))
        {
            player.PositionSave();

            // 임시로 작성된 회복 코드
            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                for (int i = 0; i < player.Pokemons.Length; i++)
                {
                    player.Pokemons[i]?.Heal();
                }
                _currentLog = "포켓몬들이 건강해졌다!";
                //_playerState = PlayerState.SkipText;
            }
        }
        else if (healer.Position == player.Position)
        {
            player.PositionLoad();
        }

        //switch (_playerState)
        //{
        //    case PlayerState.SelectAction:
        //        break;

        //    case PlayerState.SkipText:
        //    case PlayerState.Move:
        //        if (Input.IsKeyDown(ConsoleKey.Enter))
        //            ProcessNextState();
        //        break;
        //}

        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            PlayAgainRequested?.Invoke();
        }
    }

    //private void ProcessNextState()
    //{
    //    if (_playerState == PlayerState.SelectAction)
    //    {

    //    }
    //    else if (_playerState == PlayerState.SkipText)
    //    {
    //        if (Input.IsKeyDown(ConsoleKey.Enter))
    //        {
    //            _playerState = PlayerState.Move;
    //        }
    //    }
    //    else
    //    {
    //        _currentLog = string.Empty;
    //    }
    //}
    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        buffer.WriteText(65, 15, _currentLog, ConsoleColor.White);
    }
}

public enum PlayerState
{
    SelectAction,
    SkipText,
    Move
}