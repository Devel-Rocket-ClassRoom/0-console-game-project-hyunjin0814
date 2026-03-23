using Framework.Engine;

class BattleScene : Scene
{
    private Player player;
    private Enemy enemy;

    private BattleState _currentState = BattleState.Menu;
    private BattleState _previousState;

    private int _selectedMenuIndex = 0;
    private int _selectedSkillIndex = 0;
    private int _selectedChangeIndex = 0;

    private int _playerPokemonIndex = 0;
    private int _enemyPokemonIndex = 0;

    private string _currentLog = string.Empty;
    private string[] actions = { "스킬 사용", "포켓몬 교체" };

    private bool _isFirstAttackerFinished = false;

    public event GameAction ReturnRequested;

    public override void Load()
    {
        // 저장된 정보를 불러와서 배틀 시작
        player = DataManager.LoadData();
        player.StartBattle();
        AddGameObject(player);

        enemy = DataManager.LoadEnemyData();
        enemy.StartBattle();
        AddGameObject(enemy);

        _playerPokemonIndex = FindFirstAlivePokemon();

        _currentState = BattleState.Menu;
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        switch (_currentState)
        {
            case BattleState.Menu: // 메인 메뉴
                UpdateCursor(ref _selectedMenuIndex, actions.Length - 1, () => {
                    if (_selectedMenuIndex == 0) _currentState = BattleState.SelectAction;
                    else if (_selectedMenuIndex == 1) _currentState = BattleState.ChangeAction;
                });
                break;

            case BattleState.SelectAction: // 스킬 선택 메뉴
                UpdateCursor(ref _selectedSkillIndex, 4, () => {
                    if (_selectedSkillIndex == 4) _currentState = BattleState.Menu;
                    else
                    {
                        _isFirstAttackerFinished = false;

                        if (player.Pokemons[_playerPokemonIndex].Speed >= enemy.Pokemons[_enemyPokemonIndex].Speed)
                        {
                            ExecutePlayerTurn();
                        }
                        else
                        {
                            ExecuteEnemyTurn();
                        }
                    }
                    
                }, () => _currentState = BattleState.Menu);
                break;

            case BattleState.ChangeAction: // 포켓몬 교체 메뉴
                bool isForcedChange = player.Pokemons[_playerPokemonIndex].IsDead;

                UpdateCursor(ref _selectedChangeIndex, 3, () =>
                {
                    if (_selectedChangeIndex == 3)
                    {
                        if (isForcedChange)
                        {
                            _currentLog = "포켓몬을 교체해야합니다!";
                        }
                        else
                        {
                            _currentState = BattleState.Menu;
                        }
                    }
                    else TrySwitchPokemon(_selectedChangeIndex);
                }
                , () => { if (!isForcedChange) _currentState = BattleState.Menu; }
                );
                break;

            case BattleState.PlayerAttack:
            case BattleState.EnemyAttack:
            case BattleState.BattleEnd:
            case BattleState.EnemySwitchPokemon:
            case BattleState.SkipText:
                if (Input.IsKeyDown(ConsoleKey.Enter))
                    ProcessNextState();
                break;
        }
    }

    // 메뉴 선택창과 커서 출력
    private void UpdateCursor(ref int index, int max, Action onConfirm, Action onCancel = null)
    {
        if (Input.IsKeyDown(ConsoleKey.UpArrow))
            index = Math.Max(0, index - 1);
        if (Input.IsKeyDown(ConsoleKey.DownArrow))
            index = Math.Min(max, index + 1);

        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            onConfirm?.Invoke();
        }

        if (Input.IsKeyDown(ConsoleKey.Escape))
        {
            onCancel?.Invoke();
        }
    }

    private void TrySwitchPokemon(int targetIndex)
    {
        // 선택한 인덱스의 포켓몬 가져오기
        var targetMon = player.Pokemons[targetIndex];

        // 1. 현재 싸우고 있는 포켓몬인지 확인
        if (targetIndex == _playerPokemonIndex)
        {
            _currentLog = "이미 싸우고 있는 포켓몬입니다!";
            return;
        }

        // 2. 체력이 있는지 확인
        if (targetMon.IsDead)
        {
            _currentLog = $"{targetMon.Name}은(는) 기절해서 나갈 수 없다!";
            return;
        }

        bool wasForced = player.Pokemons[_playerPokemonIndex].IsDead;

        // 3. 교체 성공
        _playerPokemonIndex = targetIndex;
        _currentLog = $"{targetMon.Name}(으)로 교체했다!";

        // 4. 강제 교체면 다음 행동 선택, 선택 교체면 상대 공격 턴
        _currentState = BattleState.SkipText;

        _isFirstAttackerFinished = !wasForced;
        _previousState = wasForced ? BattleState.EnemyAttack : BattleState.ChangeAction;
    }

    // 해당 코드에서 현재 _currentState의 값을 보고 서로 턴을 주고 받음
    private void ProcessNextState()
    {
        switch (_currentState)
        {
            case BattleState.PlayerAttack:
                // 현재 적의 포켓몬 죽었는지 확인
                if (enemy.Pokemons[_enemyPokemonIndex].IsDead)
                {
                    // 다음 적 포켓몬이 있는지 체크
                    if (_enemyPokemonIndex + 1 < enemy.Pokemons.Length && enemy.Pokemons[_enemyPokemonIndex + 1] != null)
                    {
                        _previousState = BattleState.PlayerAttack; // 플레이어 공격으로 교체됐음을 기록
                        _currentLog = $"적 {enemy.Pokemons[_enemyPokemonIndex].Name}(이)가 쓰러졌다! 다음 포켓몬이 나옵니다.";
                        _currentState = BattleState.EnemySwitchPokemon; // 인덱스를 넘기기 전 대기 상태
                    }
                    else
                    {
                        _currentLog = "전투에서 승리했다! 포켓몬들의 레벨이 올랐다!";
                        int levelGap = (enemy.Name == "웅이") ? 1 : 2;
                        foreach (var p in player.Pokemons)
                        {
                            if (p != null)
                            {
                                p.LevelUp(levelGap);
                            }
                        }
                        enemy.IsDefeated = true;
                        _currentState = BattleState.BattleEnd;
                    }
                }
                else
                {
                    if (!_isFirstAttackerFinished)
                    {
                        _isFirstAttackerFinished = true;
                        ExecuteEnemyTurn();
                    }
                    else
                    {
                        _currentState = BattleState.Menu;
                        _currentLog = "무엇을 할까?";
                    }
                }
                break;

            case BattleState.EnemyAttack:
                // 현재 플레이어의 포켓몬 죽었는지 확인
                if (player.Pokemons[_playerPokemonIndex].IsDead)
                {
                    // 다른 살아있는 포켓몬이 있는지 체크
                    bool hasAlivePokemon = player.Pokemons.Any(m => m != null && !m.IsDead);

                    if (hasAlivePokemon)
                    {
                        _previousState = BattleState.EnemyAttack; // 적의 공격으로 교체됐음을 기록
                        _currentLog = $"{player.Pokemons[_playerPokemonIndex].Name}(이)가 쓰러졌다! 교체할 포켓몬을 선택하세요.";
                        _currentState = BattleState.ChangeAction; // 인덱스 넘기기 전 대기 상태
                    }
                    else
                    {
                        _currentLog = "눈앞이 캄캄해졌다...";
                        _currentState = BattleState.BattleEnd;
                    }
                }
                else
                {
                    if (!_isFirstAttackerFinished)
                    {
                        _isFirstAttackerFinished = true;
                        ExecutePlayerTurn();
                    }    
                    else
                    {
                        _currentLog = "무엇을 할까?";
                        _currentState = BattleState.Menu;
                    }
                }
                break;

            case BattleState.EnemySwitchPokemon:
                // 적 포켓몬 교체 연출 직후 실행
                if (_previousState == BattleState.PlayerAttack)
                {
                    _enemyPokemonIndex++; // 적 다음 포켓몬으로 교체
                    _currentLog = $"상대는 {enemy.Pokemons[_enemyPokemonIndex].Name}(을)를 꺼냈다! 무엇을 할까?";
                    _currentState = BattleState.Menu;
                }
                break;

            case BattleState.SkipText:
                // 텍스트 스킵용 로직
                if (_previousState == BattleState.ChangeAction)
                {
                    ExecuteEnemyTurn();
                }
                else
                {
                    _currentLog = "무엇을 할까?";
                    _currentState = BattleState.Menu;
                }
                break;

            case BattleState.BattleEnd:
                DataManager.SaveData(player);
                foreach (var p in enemy.Pokemons) p?.Heal();
                //DataManager.SaveEnemyData(enemy);
                ReturnRequested?.Invoke();
                break;
        }
    }

    // 플레이어가 선택한 스킬의 데미지를 주고 출력
    private void ExecutePlayerTurn()
    {
        float multiplier;
        bool isHit;

        int previousHp = enemy.Pokemons[_enemyPokemonIndex].CurrentHp;
        var skill = player.Pokemons[_playerPokemonIndex].skills[_selectedSkillIndex];
        int damage = player.Pokemons[_playerPokemonIndex].AttackTo(skill, enemy.Pokemons[_enemyPokemonIndex], out multiplier, out isHit);
        enemy.Pokemons[_enemyPokemonIndex].TakeDamage(damage);

        if (!isHit)
        {
            _currentLog = $"{player.Pokemons[_playerPokemonIndex].Name}의 {skill.Name}! 하지만 공격은 빗나갔다!";
        }
        else
        {
            _currentLog = $"{player.Pokemons[_playerPokemonIndex].Name}의 {skill.Name}! {previousHp - enemy.Pokemons[_enemyPokemonIndex].CurrentHp}의 피해!";
            if (multiplier >= 2.0f)
            {
                _currentLog += " 효과가 굉장했다!";
            }
            else if (multiplier <= 0.5f && multiplier > 0)
            {
                _currentLog += " 효과가 별로인 듯하다....";
            }
            else if (multiplier == 0)
            {
                _currentLog += " 효과가 없는 듯하다...";
            }
        }
        _currentState = BattleState.PlayerAttack;
    }

    // 적이 랜덤으로 스킬을 사용하여 데미지를 받음
    private void ExecuteEnemyTurn()
    {
        Random rand = new Random();
        float multiplier;
        bool isHit;

        int previousHp = player.Pokemons[_playerPokemonIndex].CurrentHp;
        var skill = enemy.Pokemons[_enemyPokemonIndex].skills[rand.Next(0, 4)];
        int damage = enemy.Pokemons[_enemyPokemonIndex].AttackTo(skill, player.Pokemons[_playerPokemonIndex], out multiplier, out isHit);
        player.Pokemons[_playerPokemonIndex].TakeDamage(damage);

        if (!isHit)
        {
            _currentLog = $"{player.Pokemons[_playerPokemonIndex].Name}의 {skill.Name}! 하지만 공격은 빗나갔다!";
        }
        else
        {
            _currentLog = $"적 {enemy.Pokemons[_enemyPokemonIndex].Name}의 {skill.Name}! {previousHp - player.Pokemons[_playerPokemonIndex].CurrentHp}의 피해!";
            if (multiplier >= 2.0f)
            {
                _currentLog += " 효과가 굉장했다!";
            }
            else if (multiplier <= 0.5f && multiplier > 0)
            {
                _currentLog += " 효과가 별로인 듯하다....";
            }
            else if (multiplier == 0)
            {
                _currentLog += " 효과가 없는 듯하다...";
            }
        }
        _currentState = BattleState.EnemyAttack;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        DrawPokemonInfo(buffer, player.Pokemons[_playerPokemonIndex], isPlayer: true);
        DrawPokemonInfo(buffer, enemy.Pokemons[_enemyPokemonIndex], isPlayer: false);

        switch (_currentState)
        {
            case BattleState.Menu:
                DrawMenu(buffer, "행동 선택", actions, _selectedMenuIndex);
                break;
            case BattleState.SelectAction:
                var skills = player.Pokemons[_playerPokemonIndex].skills;
                string typeSkill;

                string[] skillNames = new string[5];
                for (int i = 0; i < 4; i++)
                {
                    skillNames[i] = skills[i].Name;
                }
                skillNames[4] = "취소";
                DrawMenu(buffer, "기술 선택", skillNames, _selectedSkillIndex);
                break;
            case BattleState.ChangeAction:
                string[] monNames = new string[4];
                for (int i = 0; i < 3; i++)
                {
                    var mon = player.Pokemons[i];
                    monNames[i] = mon != null ? $"{mon.Name} (HP: {mon.CurrentHp}/{mon.MaxHp})" : "(비어있음)";
                }
                monNames[3] = "취소";
                DrawMenu(buffer, "교체 선택", monNames, _selectedChangeIndex);
                break;
        }
        buffer.WriteTextCentered(25, _currentLog, ConsoleColor.White);
    }

    private void DrawMenu(ScreenBuffer buffer, string header, string[] list, int selectedIndex)
    {
        int x = 35; // 목록이 그려질 X 좌표 (포켓몬 아트 옆)
        int y = 10; // 목록이 시작될 Y 좌표

        buffer.WriteText(x, y - 2, $"==== [{header}] ====", ConsoleColor.Gray);

        for (int i = 0; i < list.Length; i++)
        {
            string prefix;
            ConsoleColor color;

            // 현재 선택된 인덱스인 경우 커서 표시 및 색상 변경
            if (i == selectedIndex)
            {
                prefix = "> ";
                color = ConsoleColor.Yellow;
            }
            else
            {
                prefix = "  ";
                color = ConsoleColor.White;
            }

            buffer.WriteText(x, y + i, $"{prefix}{i + 1}. {list[i]}", color);
        }
        buffer.WriteText(x, y + list.Length, "=====================", ConsoleColor.Gray);
    }

    // bool isPlayer는 패턴 매칭으로 변경 예정 (테스트용)
    private void DrawPokemonInfo(ScreenBuffer buffer, Pokemon mon, bool isPlayer)
    {
        int x = isPlayer ? 5 : 45; // 플레이어는 왼쪽, 적은 오른쪽에 배치
        int y = isPlayer ? 16 : 1;

        buffer.WriteText(x, y, $"[{mon.Name}] Lv.{mon.Level} {mon.CurrentHp}/{mon.MaxHp}", isPlayer ? ConsoleColor.Cyan : ConsoleColor.Red);
        buffer.WriteText(x, y + 1, " /\\_/\\");
        buffer.WriteText(x, y + 2, "( o.o )");
        buffer.WriteText(x, y + 3, " > ^ <");
    }

    // 처음 내보낼 포켓몬을 찾음
    private int FindFirstAlivePokemon()
    {
        for (int i = 0; i < player.Pokemons.Length; i++)
        {
            
            if (player.Pokemons[i] != null && !player.Pokemons[i].IsDead)
            {
                return i; 
            }
        }
        return -1;
    }
}

public enum BattleState
{
    Menu,
    SelectAction,
    ChangeAction,
    PlayerAttack,   
    EnemyAttack,    
    EnemySwitchPokemon,
    SkipText,
    BattleEnd       
}