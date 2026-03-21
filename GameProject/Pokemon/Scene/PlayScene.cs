using Framework.Engine;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

public class PlayScene : Scene
{
    private Wall wall;
    private Player player;
    private Trainer npc;
    private Healer healer;
    private string _currentLog = string.Empty;

    private PlayerState _playerState = PlayerState.Move;
    private int _selectedChoiceIndex = 0;
    private string[] _choices = { "예", "아니오" };
    private enum InteractionTarget { None, NPC, Healer }
    private InteractionTarget _currentTarget = InteractionTarget.None;

    public event GameAction BattleRequested;
    public event GameAction PlayAgainRequested;

    public override void Load()
    {
        // 만약 저장된 정보가 있으면 불러오고 개체를 새로 생성하지 않음
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

        _playerState = PlayerState.Move;
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        switch (_playerState)
        {
            case PlayerState.Move:
                player.PositionSave();
                UpdateGameObjects(deltaTime);
                UpdateMove(deltaTime);
                break;

            case PlayerState.SelectAction:
                UpdateSelection();
                break;

            case PlayerState.SkipText:
                if (Input.IsKeyDown(ConsoleKey.Enter))
                {
                    _currentLog = string.Empty;
                    _playerState = PlayerState.Move;
                }
                break;
        }

        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            PlayAgainRequested?.Invoke();
        }
    }

    private void UpdateMove(float deltaTime)
    {
        // 위치 중복 방지
        if (player.Position == npc.Position || player.Position == healer.Position)
        {
            player.PositionLoad();
            return;
        }

        // 상호작용 확인
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            // NPC 상호작용
            if (player.IsInBounds(npc.Position.X, npc.Position.Y))
            {
                _playerState = PlayerState.SelectAction;
                _currentTarget = InteractionTarget.NPC;
                _currentLog = "전투를 시작하시겠습니까?";
                _selectedChoiceIndex = 0;
            }
            // 힐러 상호작용
            else if (player.IsInBounds(healer.Position.X, healer.Position.Y))
            {
                _playerState = PlayerState.SelectAction;
                _currentTarget = InteractionTarget.Healer;
                _currentLog = "체력을 회복하시겠습니까?";
                _selectedChoiceIndex = 0;
            }
        }
    }

    // 상호작용 확인 (Enter 누를 때)
    private void OnInteractionConfirm()
    {
        // 아니오 선택
        if (_selectedChoiceIndex != 0) 
        {
            _currentLog = string.Empty;
            _playerState = PlayerState.Move;
            return;
        }

        // 예 선택
        if (_currentTarget == InteractionTarget.NPC) // Trainer NPC인 경우
        {
            DataManager.SaveData(player);
            BattleRequested?.Invoke();
        }
        else if (_currentTarget == InteractionTarget.Healer) // Healer NPC인 경우
        {
            foreach (var p in player.Pokemons) p?.Heal();
            _currentLog = "포켓몬들이 건강해졌다!";
            _playerState = PlayerState.SkipText;
        }
    }

    // 상호작용 취소 (ESC 누를 때)
    private void CancelSelection()
    {
        _currentLog = string.Empty;
        _playerState = PlayerState.Move;
    }

    // 선택지 상태 갱신 로직
    private void UpdateSelection()
    {
        UpdateCursor(ref _selectedChoiceIndex, _choices.Length - 1, OnInteractionConfirm, CancelSelection);
    }

    // 커서 갱신 로직
    private void UpdateCursor(ref int index, int max, Action onConfirm, Action onCancel = null)
    {
        if (Input.IsKeyDown(ConsoleKey.UpArrow))
            index = Math.Max(0, index - 1);
        if (Input.IsKeyDown(ConsoleKey.DownArrow))
            index = Math.Min(max, index + 1);

        if (Input.IsKeyDown(ConsoleKey.Enter))
            onConfirm?.Invoke();

        if (Input.IsKeyDown(ConsoleKey.Escape))
            onCancel?.Invoke();
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        buffer.WriteText(65, 15, _currentLog, ConsoleColor.White);

        if (_playerState == PlayerState.SelectAction)
        {
            DrawChoiceMenu(buffer);
        }
    }
    
    // 메뉴창 그리는 로직
    private void DrawChoiceMenu(ScreenBuffer buffer)
    {
        int x = 65;
        int y = 17;
        for (int i = 0; i < _choices.Length; i++)
        {
            string prefix;
            ConsoleColor color; 

            // 선택되면 커서 "> "로 표시
            if (i == _selectedChoiceIndex)
            {
                prefix = "> ";
                color = ConsoleColor.Yellow;
            }
            else
            {
                prefix = "  ";
                color = ConsoleColor.White;
            }
                buffer.WriteText(x, y + i, $"{prefix}{_choices[i]}", color);
        }
    }
}

public enum PlayerState
{
    SelectAction,
    SkipText,
    Move
}