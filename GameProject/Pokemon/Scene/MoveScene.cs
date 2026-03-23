using Framework.Engine;

public class MoveScene : Scene
{
    private Wall wall;
    private Player player;
    private List<Enemy> _npcs;
    private Healer healer;
    private Giver professor;
    private Stair stair;
    private string _currentLog = string.Empty;

    private PlayerState _playerState = PlayerState.Move;
    private int _selectedChoiceIndex = 0;
    private int _selectedPokemonIndex = 0;
    private string[] _choices = { "예", "아니오" };

    private int _tutorialTextIndex = 0; // 대화 넘기기용 (0~2)
    private int _offerPokemonIndex = 0; // 어떤 포켓몬을 제안 중인지
    private int _acquiredCount = 0;     // 현재까지 받은 포켓몬 수
    private string[] _tutorialLines = {
    "허허, 자네가 타워를 오르겠다고?",
    "이 위는 아주 위험하다네. 포켓몬 없이 갈 순 없지.",
    "여기 있는 포켓몬들 중 3마리를 골라보게나."
    };
    private List<string> _starterNames = new List<string>{ "파이리", "꼬부기", "이상해씨", "이브이", "푸린" };

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
            player = new Player(this, 1, 28, "Player");
        }
        AddGameObject(player);

        wall = new Wall(this);
        AddGameObject(wall);

        // DataManager에 저장된 리스트 있는지 확인
        if (DataManager.npcList != null)
        {
            _npcs = DataManager.npcList;

            foreach (var e in _npcs)
            {
                e.ExitBattle();
            }
        }
        else
        {
            _npcs = new List<Enemy>();

            Enemy npc1 = new Enemy(this, 30, 1, "웅이");
            npc1.GetPokemons(PokemonList.GetPokemons1());
            _npcs.Add(npc1);

            Enemy npc2 = new Enemy(this, 58, 14, "이슬");
            npc2.GetPokemons(PokemonList.GetPokemons2());
            _npcs.Add(npc2);

            Enemy npc3 = new Enemy(this, 30, 28, "레드");
            npc3.GetPokemons(PokemonList.GetPokemons3());
            _npcs.Add(npc3);

            DataManager.SaveNPCList(_npcs);
        }

        healer = new Healer(this, 1, 14, "Healer");
        AddGameObject(healer);

        professor = new Giver(this, 30, 28, "오박사");
        AddGameObject(professor);

        stair = new Stair(this);
        AddGameObject(stair);

        _playerState = PlayerState.Move;
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        if (_npcs != null && DataManager.currentStage >= _npcs.Count)
        {
            if (Input.IsKeyDown(ConsoleKey.Escape))
            {
                DataManager.ResetGameData();

                PlayAgainRequested?.Invoke(); // 타이틀 씬으로 돌아가는 이벤트 발생
            }
            return;
        }

        if (_npcs != null && _npcs[DataManager.currentStage].IsDefeated)
        {
            stair.IsActive = true;
        }

        if (professor != null)
        {
            professor.IsActive = (DataManager.currentStage == 0);
        }

        switch (_playerState)
        {
            case PlayerState.Move:
                player.PositionSave();
                UpdateGameObjects(deltaTime);
                UpdateMove(deltaTime);
                CheckStairCollision();
                if (Input.IsKeyDown(ConsoleKey.Tab))
                {
                    _playerState = PlayerState.Inventory;
                    _selectedPokemonIndex = 0;
                    _currentLog = "포켓몬 정보를 확인합니다. (ESC: 닫기)";
                }
                break;

            case PlayerState.TutorialText:
                if (Input.IsKeyDown(ConsoleKey.Enter))
                {
                    _tutorialTextIndex++;
                    if (_tutorialTextIndex < _tutorialLines.Length)
                    {
                        _currentLog = _tutorialLines[_tutorialTextIndex];
                    }
                    else
                    {
                        // 대화가 끝나면 포켓몬 선택 시작
                        StartPokemonOffer();
                    }
                }
                break;

            case PlayerState.TutorialSelect:
                UpdateTutorialSelection();
                break;

            case PlayerState.Inventory:
                UpdateCursor(ref _selectedPokemonIndex, player.Pokemons.Length - 1,
                    OnPokemonSelect, // Enter 시 실행
                    () => { _playerState = PlayerState.Move; _currentLog = ""; } // ESC 시 복귀
                );
                break;

            case PlayerState.PokemonDetail:
                if (Input.IsKeyDown(ConsoleKey.Enter) || Input.IsKeyDown(ConsoleKey.Escape))
                {
                    _playerState = PlayerState.Inventory;

                    _currentLog = "포켓몬 목록을 확인합니다. (ESC: 닫기)";
                }
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

        //if (Input.IsKeyDown(ConsoleKey.Escape))
        //{
        //    PlayAgainRequested?.Invoke();
        //}
    }

    private void UpdateMove(float deltaTime)
    {
        Enemy activeEnemy = _npcs[DataManager.currentStage];

        // 위치 중복 방지
        if (player.Position == activeEnemy.Position || player.Position == healer.Position)
        {
            player.PositionLoad();
            return;
        }

        // 튜토리얼 NPC 따로 관리
        if (DataManager.currentStage == 0 && professor.IsActive)
        {
            if (player.Position == professor.Position)
            {
                player.PositionLoad();
                return;
            }

            if (Input.IsKeyDown(ConsoleKey.Enter))
            {
                if (player.IsInBounds(professor.Position.X, professor.Position.Y))
                {
                    if (!DataManager.isTutorialDone)
                    {
                        _playerState = PlayerState.TutorialText;
                        _tutorialTextIndex = 0;
                        _currentLog = _tutorialLines[0];
                    }
                    else
                    {
                        _currentLog = "오박사: 자네의 도전을 응원하네!";
                        _playerState = PlayerState.SkipText;
                    }
                }
            }
        }

        // 상호작용 확인
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            // NPC 상호작용
            if (player.IsInBounds(activeEnemy.Position.X, activeEnemy.Position.Y))
            {
                if (!activeEnemy.IsDefeated)
                {
                    int pokemonCount = 0;
                    for (int i = 0; i < player.Pokemons.Length; i++)
                    {
                        if (player.Pokemons[i] != null)
                        {
                            pokemonCount++;
                        }
                    }

                    if (pokemonCount < 3)
                    {
                        _currentLog = "아직 보유한 포켓몬이 부족해. (최소 3마리 필요)";
                        _playerState = PlayerState.SkipText;
                        return; // 더 이상 배틀 선택창 로직을 타지 않도록 종료
                    }
                    else
                    {
                        _playerState = PlayerState.SelectAction;
                        _currentTarget = InteractionTarget.NPC;
                        _currentLog = $"{activeEnemy.Name}: 전투를 시작하시겠습니까?";
                        _selectedChoiceIndex = 0;
                    }
                }
                else
                {
                    _currentLog = $"{activeEnemy.Name}: 우측 계단을 통해 다음 층으로 올라가!";
                    _playerState = PlayerState.SkipText; // Enter를 누르면 Move로 복구됨
                }
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

    private void StartPokemonOffer()
    {
        _playerState = PlayerState.TutorialSelect;
        _selectedChoiceIndex = 0;
        _currentLog = $"{_starterNames[_offerPokemonIndex]}를 가져가겠나?";
    }

    private void UpdateTutorialSelection()
    {
        // 메뉴 커서 이동 (기존 UpdateSelection 로직 활용)
        if (Input.IsKeyDown(ConsoleKey.UpArrow)) _selectedChoiceIndex = 0;
        if (Input.IsKeyDown(ConsoleKey.DownArrow)) _selectedChoiceIndex = 1;

        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            if (_selectedChoiceIndex == 0) // "예" 선택
            {
                string targetName = _starterNames[_offerPokemonIndex];
                Pokemon template = null;

                switch (targetName)
                {
                    case "꼬부기": template = PokemonList.bugi; break;
                    case "이상해씨": template = PokemonList.sseed; break;
                    case "파이리": template = PokemonList.pairi; break;
                    case "푸린": template = PokemonList.purin; break;
                    case "이브이": template = PokemonList.eevee; break;
                }

                player.Pokemons[_acquiredCount] = template;
                _starterNames.RemoveAt(_offerPokemonIndex);
                _acquiredCount++;

                if (_acquiredCount < 3)
                {
                    _currentLog = $"{targetName}을 얻었다! 다음 포켓몬을 골라보게.";
                    if (_offerPokemonIndex >= _starterNames.Count)
                    {
                        _offerPokemonIndex = 0;
                    }
                    _playerState = PlayerState.SkipText;
                }
                else
                {
                    _currentLog = "오박사: 좋아, 3마리 모두 골랐군! 이제 위로 올라가게나.";
                    DataManager.isTutorialDone = true;
                    DataManager.SaveData(player);
                    _playerState = PlayerState.SkipText;
                }
            }
            else // "아니오" 선택
            {
                _offerPokemonIndex = (_offerPokemonIndex + 1) % _starterNames.Count;
                _currentLog = $"그러면... {_starterNames[_offerPokemonIndex]}는 어떤가?";
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
        if (_currentTarget == InteractionTarget.NPC) // Enemy NPC인 경우
        {
            DataManager.SaveEnemyData(_npcs[DataManager.currentStage]);
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

    private void CheckStairCollision()
    {
        // 계단이 활성화되어 있고, 플레이어가 계단 위치(55, 2)에 도달했을 때
        if (stair != null && stair.IsActive && player.Position.X == 58 && player.Position.Y == 1)
        {
            DataManager.currentStage++; // 다음 층으로 스테이지 번호 증가
            stair.IsActive = false; // 새로운 층에서는 계단을 다시 비활성화

            if (DataManager.currentStage < _npcs.Count)
            {
                // [NEW] 플레이어 위치를 시작 지점(좌측 하단)으로 리셋
                player.SetPosition(1, 28);
                _currentLog = $"{DataManager.currentStage + 1}층에 도달했습니다! 새로운 적이 나타납니다.";
                _playerState = PlayerState.SkipText; // 안내 문구를 읽을 수 있게 SkipText 상태로 전환
            }
            else
            {
                // 모든 NPC를 격파하고 마지막 계단을 탔을 때
                _currentLog = "축하합니다! 모든 층의 트레이너를 이겨 챔피언이 되었습니다!";
                _playerState = PlayerState.SkipText;
            }
        }
    }

    // 선택지 상태 갱신 로직
    private void UpdateSelection()
    {
        UpdateCursor(ref _selectedChoiceIndex, _choices.Length - 1, OnInteractionConfirm, CancelSelection);
    }

    // 포켓몬 정보 확인 로직
    private void OnPokemonSelect()
    {
        var selected = player.Pokemons[_selectedPokemonIndex];
        if (selected == null) return;

        _playerState = PlayerState.PokemonDetail;
        _currentLog = $"{selected.Name}의 상세 정보입니다.";
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

        if (DataManager.currentStage < _npcs.Count)
        {
            Enemy activeEnemy = _npcs[DataManager.currentStage];
            activeEnemy.Draw(buffer); 
        }

        professor.Draw(buffer);

        buffer.WriteText(65, 20, _currentLog, ConsoleColor.White);
        buffer.WriteText(10, 33, "Tap: 메뉴", ConsoleColor.DarkGray);
        buffer.WriteText(10, 34, "Enter: 선택", ConsoleColor.DarkGray);
        if (_playerState == PlayerState.Inventory || _playerState == PlayerState.PokemonDetail)
        {
            DrawPokemonList(buffer);
        }

        if (_playerState == PlayerState.PokemonDetail)
        {
            DrawPokemonStatsAndSkills(buffer);
        }

        if (_playerState == PlayerState.SelectAction || _playerState == PlayerState.TutorialSelect)
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

    private void DrawPokemonList(ScreenBuffer buffer)
    {
        int startX = 65;
        int startY = 2;
        buffer.WriteText(startX, startY, "=== [포켓몬 목록] ===", ConsoleColor.Cyan);

        for (int i = 0; i < player.Pokemons.Length; i++)
        {
            var p = player.Pokemons[i];
            if (p == null) continue;

            string prefix = (i == _selectedPokemonIndex) ? "> " : "  ";
            ConsoleColor color = (i == _selectedPokemonIndex) ? ConsoleColor.Yellow : ConsoleColor.White;
            buffer.WriteText(startX, startY + 1 + i, $"{prefix}{p.Name}", color);
        }
    }

    private void DrawPokemonStatsAndSkills(ScreenBuffer buffer)
    {
        var p = player.Pokemons[_selectedPokemonIndex];
        int x = 65;
        int y = 6;

        string typePokemon = p.Type switch
        {
            PokemonType.Water => "물",
            PokemonType.FIre => "불",  
            PokemonType.Grass => "풀",
            PokemonType.Normal => "노말",
            _ => "알 수 없음"
        };

        // 1. 기본 능력치 출력
        buffer.WriteText(x, y, "----------------------", ConsoleColor.Gray);
        buffer.WriteText(x, y + 1, $"체력: {p.CurrentHp}/{p.MaxHp} 레벨: {p.Level} 공격: {p.AttackPower} 방어: {p.Defense} 속도: {p.Speed}, 타입: {typePokemon}", ConsoleColor.Green);

        // 2. 스킬 목록 출력
        buffer.WriteText(x, y + 3, "[소유 스킬]", ConsoleColor.Cyan);
        for (int i = 0; i < p.skills.Length; i++) // Skills가 List<Skill> 형태라고 가정
        {
            var s = p.skills[i];
            string typeSkill = s.Type switch
            {
                PokemonType.Water => "물",
                PokemonType.FIre => "불",
                PokemonType.Grass => "풀",
                PokemonType.Normal => "노말",
                _ => "알 수 없음"
            };
            buffer.WriteText(x, y + 4 + (i * 2), $"- {s.Name} (위력:{s.PowerRate} 명중:{s.HitRate}) [{typeSkill}]", ConsoleColor.White);
            buffer.WriteText(x, y + 5 + (i * 2), $"  : {s.Description}", ConsoleColor.DarkGray);
        }
    }
}

public enum PlayerState
{
    SelectAction,
    SkipText,
    Move,
    Inventory,
    TutorialText,
    TutorialSelect,
    PokemonDetail
}