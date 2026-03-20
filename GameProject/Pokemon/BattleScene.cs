using System;
using Framework.Engine;

class BattleScene : Scene
{
    private Player player;
    private Trainer enemy;
    private BattleState _currentState = BattleState.SelectAction;
    private BattleState _previousState;
    private int _selectedSkillIndex = 0;
    private int _playerPokemonIndex = 0;
    private int _enemyPokemonIndex = 0;
    private string _currentLog = string.Empty;

    public event GameAction ReturnRequested;

    public override void Load()
    {
        // 저장된 정보를 통해 개체를 생성 혹은 불러와서 배틀 시작 하는 코드 작성 필요
        player = DataManager.LoadData();
        player.StartBattle();
        AddGameObject(player);

        enemy = new Trainer(this, 29, 4, "NPC1");
        enemy.StartBattle();
        AddGameObject(enemy);
        Pokemon pokemon2 = new Pokemon("파이리1", PokemonType.FIre, 20, 10, 3, 3);
        pokemon2.GetSkills(SkillChart.fires[2], SkillChart.fires[1], SkillChart.normals[2], SkillChart.normals[1]);
        enemy.GetPokemon0(pokemon2);
        Pokemon pokemon3 = new Pokemon("파이리2", PokemonType.FIre, 15, 10, 2, 2);
        pokemon3.GetSkills(SkillChart.fires[0], SkillChart.fires[1], SkillChart.normals[0], SkillChart.normals[1]);
        enemy.GetPokemon1(pokemon3);

        _currentState = BattleState.SelectAction;
    }

    public override void Unload()
    {
        ClearGameObjects();
    }

    public override void Update(float deltaTime)
    {
        switch (_currentState)
        {
            case BattleState.SelectAction:
                HandleInput();
                break;

            case BattleState.PlayerAttack:
            case BattleState.EnemyAttack:
            case BattleState.BattleEnd:
            case BattleState.ChangePokemon:
            case BattleState.SkipText:
                if (Input.IsKeyDown(ConsoleKey.Enter))
                    ProcessNextState();
                break;
        }

        //if (Input.IsKeyDown(ConsoleKey.Escape)) ReturnRequested?.Invoke();
    }

    // 선택지 인덱스 0~3으로 조정
    private void HandleInput()
    {
        _currentLog = "무엇을 할까?";

        if (Input.IsKeyDown(ConsoleKey.UpArrow))
        {
            _selectedSkillIndex = Math.Max(0, _selectedSkillIndex - 1);
        }
        if (Input.IsKeyDown(ConsoleKey.DownArrow))
        {
            _selectedSkillIndex = Math.Min(3, _selectedSkillIndex + 1);
        }
        if (Input.IsKeyDown(ConsoleKey.Enter))
        {
            ExecutePlayerTurn();
        }
    }

    // 해당 코드에서 현재 _currentState의 값을 보고 서로 턴을 주고 받음
    private void ProcessNextState()
    {
        if (_currentState == BattleState.PlayerAttack)
        {
            if (enemy.Pokemons[_enemyPokemonIndex].IsDead)
            {
                if (enemy.Pokemons[_enemyPokemonIndex + 1] != null)
                {
                    _currentLog = $"{enemy.Pokemons[_enemyPokemonIndex].Name}이(가) 쓰러졌다! (교체)";
                    _previousState = BattleState.PlayerAttack;
                    _currentState = BattleState.ChangePokemon;
                }
                else
                {
                    _currentLog = $"{enemy.Pokemons[_enemyPokemonIndex].Name}이(가) 쓰러졌다! 승리했다!";
                    _currentState = BattleState.BattleEnd;
                }
            }
            else
            {
                ExecuteEnemyTurn();
            }
        }
        else if (_currentState == BattleState.EnemyAttack)
        {
            if (player.Pokemons[_playerPokemonIndex].IsDead)
            {
                bool anyAlive = false;
                foreach (var myMon in player.Pokemons)
                {
                    if (myMon == null)
                    {
                        break;
                    }
                    if (!myMon.IsDead)
                    {
                        anyAlive = true;
                        break;
                    }
                }

                if (anyAlive)
                {
                    _currentLog = $"{player.Pokemons[_enemyPokemonIndex].Name}이(가) 쓰러졌다! (교체)";
                    _previousState = BattleState.EnemyAttack;
                    _currentState = BattleState.ChangePokemon;
                }
                else
                {
                    _currentLog = $"{player.Pokemons[_playerPokemonIndex].Name}이(가) 쓰러졌다... 패배했다.";
                    _currentState = BattleState.BattleEnd;
                }
            }
            else
            {
                _currentState = BattleState.SelectAction;
            }
        }
        else if (_currentState == BattleState.ChangePokemon)
        {
            if (_previousState == BattleState.PlayerAttack)
            {
                _enemyPokemonIndex++;
                _currentLog = $"{enemy.Pokemons[_enemyPokemonIndex].Name}이(가) 교체되어나왔다!";
            }
            else if (_previousState == BattleState.EnemyAttack)
            {
                _playerPokemonIndex++;
                _currentLog = $"{player.Pokemons[_playerPokemonIndex].Name}이(가) 교체되어나왔다!";
            }
            _currentState = BattleState.SkipText;
        }
        else if (_currentState == BattleState.SkipText)
        {
            _currentState = BattleState.SelectAction;
        }
        else if (_currentState == BattleState.BattleEnd)
        {
            // 배틀을 종료하기 전에 플레이어와 상대의 정보를 갱신해주는 코드 작성 필요
            DataManager.SaveData(player);
            ReturnRequested?.Invoke();
        }
    }

    // 플레이어가 선택한 스킬의 데미지를 주고 출력
    private void ExecutePlayerTurn()
    {
        int previousHp = enemy.Pokemons[_enemyPokemonIndex].CurrentHp;
        var skill = player.Pokemons[_playerPokemonIndex].skills[_selectedSkillIndex];
        int damage = player.Pokemons[_playerPokemonIndex].AttackTo(skill, enemy.Pokemons[_enemyPokemonIndex]);
        enemy.Pokemons[_enemyPokemonIndex].TakeDamage(damage);

        _currentLog = $"{player.Pokemons[_playerPokemonIndex].Name}의 {skill.Name}! {previousHp - enemy.Pokemons[_enemyPokemonIndex].CurrentHp}의 피해!";
        _currentState = BattleState.PlayerAttack;
    }

    // 적이 랜덤으로 스킬을 사용하여 데미지를 받음
    private void ExecuteEnemyTurn()
    {
        Random rand = new Random();
        int previousHp = player.Pokemons[_playerPokemonIndex].CurrentHp;
        var skill = enemy.Pokemons[_enemyPokemonIndex].skills[rand.Next(0, 4)];
        int damage = enemy.Pokemons[_enemyPokemonIndex].AttackTo(skill, player.Pokemons[_playerPokemonIndex]);
        player.Pokemons[_playerPokemonIndex].TakeDamage(damage);

        _currentLog = $"적 {enemy.Pokemons[_enemyPokemonIndex].Name}의 {skill.Name}! {previousHp - player.Pokemons[_playerPokemonIndex].CurrentHp}의 피해!";
        _currentState = BattleState.EnemyAttack;
    }

    private void ChageEnemyTurn()
    {
        _enemyPokemonIndex++;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        DrawPokemonInfo(buffer, player.Pokemons[_playerPokemonIndex], isPlayer: true);
        DrawPokemonInfo(buffer, enemy.Pokemons[_enemyPokemonIndex], isPlayer: false);

        if (_currentState == BattleState.SelectAction)
        {
            DrawSkillMenu(buffer);
        }

        buffer.WriteTextCentered(25, _currentLog, ConsoleColor.White);
    }

    private void DrawSkillMenu(ScreenBuffer buffer)
    {
        int x = 35; // 스킬 목록이 그려질 X 좌표 (포켓몬 아트 옆)
        int y = 10; // 스킬 목록이 시작될 Y 좌표

        buffer.WriteText(x, y - 2, "==== [기술 선택] ====", ConsoleColor.Gray);

        var myMon = player.Pokemons[_playerPokemonIndex];

        // 4개의 스킬 루프
        for (int i = 0; i < 4; i++)
        {
            var skill = myMon.skills[i];

            if (skill == null) continue; // 스킬이 없으면 건너뜀 (null 체크)

            string prefix;
            ConsoleColor color;

            // 현재 선택된 인덱스인 경우 커서 표시 및 색상 변경
            if (i == _selectedSkillIndex)
            {
                prefix = "> ";
                color = ConsoleColor.Yellow;
            }
            else
            {
                prefix = "  ";
                color = ConsoleColor.White;
            }

            // 스킬 이름과 위력 출력
            buffer.WriteText(x, y + i, $"{prefix}{i + 1}. {skill.Name} (ATK: {skill.PowerRate})", color);
        }

        buffer.WriteText(x, y + 4, "=====================", ConsoleColor.Gray);
    }

    // bool isPlayer는 패턴 매칭으로 변경 예정 (테스트용)
    private void DrawPokemonInfo(ScreenBuffer buffer, Pokemon mon, bool isPlayer)
    {
        int x = isPlayer ? 5 : 45; // 플레이어는 왼쪽, 적은 오른쪽에 배치
        int y = isPlayer ? 15 : 2;

        buffer.WriteText(x, y, $"[{mon.Name}] {mon.CurrentHp}/{mon.MaxHp}", isPlayer ? ConsoleColor.Cyan : ConsoleColor.Red);
        buffer.WriteText(x, y + 1, " /\\_/\\");
        buffer.WriteText(x, y + 2, "( o.o )");
        buffer.WriteText(x, y + 3, " > ^ <");
    }
}

public enum BattleState
{
    SelectAction,   
    PlayerAttack,   
    EnemyAttack,    
    ChangePokemon,
    SkipText,
    BattleEnd       
}