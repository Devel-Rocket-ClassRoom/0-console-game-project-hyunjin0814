using System;
using Framework.Engine;

class BattleScene : Scene
{
    private Player player;
    private Trainer enemy;
    private BattleState _currentState = BattleState.SelectAction;
    private int _selectedSkillIndex = 0;
    private string _currentLog = string.Empty;

    public event GameAction ReturnRequested;

    public override void Load()
    {
        player = new Player(this, 29, 14, "Player");
        AddGameObject(player);
        player.isBattle = true;
        Pokemon pokemon1 = new Pokemon("꼬부기", PokemonType.Water, 20, 3, 5, 2);
        pokemon1.GetSkills(SkillChart.waters[0], SkillChart.waters[1], SkillChart.normals[0], SkillChart.normals[1]);
        player.GetPokemon(pokemon1);

        enemy = new Trainer(this, 29, 4, "NPC1");
        AddGameObject(enemy);
        enemy.isBattle = true;
        Pokemon pokemon2 = new Pokemon("파이리", PokemonType.FIre, 20, 4, 3, 3);
        pokemon2.GetSkills(SkillChart.fires[0], SkillChart.fires[1], SkillChart.normals[0], SkillChart.normals[1]);
        enemy.GetPokemon(pokemon2);

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
                if (Input.IsKeyDown(ConsoleKey.Enter))
                    ProcessNextState();
                break;
        }

        if (Input.IsKeyDown(ConsoleKey.Escape)) ReturnRequested?.Invoke();
    }

    private void HandleInput()
    {
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

    private void ExecutePlayerTurn()
    {
        int previousHp = enemy.Pokemons[0].CurrentHp;
        var skill = player.Pokemons[0].skills[_selectedSkillIndex];
        int damage = player.Pokemons[0].AttackTo(skill, enemy.Pokemons[0]);
        enemy.Pokemons[0].TakeDamage(damage);

        _currentLog = $"{player.Pokemons[0].Name}의 {skill.Name}! {previousHp - enemy.Pokemons[0].CurrentHp}의 피해!";
        _currentState = BattleState.PlayerAttack;
    }

    private void ProcessNextState()
    {
        if (_currentState == BattleState.PlayerAttack)
        {
            if (enemy.Pokemons[0].IsDead)
            {
                _currentLog = $"{enemy.Pokemons[0].Name}이(가) 쓰러졌다! 승리했다!";
                _currentState = BattleState.BattleEnd;
            }
            else
            {
                ExecuteEnemyTurn();
            }
        }
        else if (_currentState == BattleState.EnemyAttack)
        {
            if (player.Pokemons[0].IsDead)
            {
                _currentLog = $"{player.Pokemons[0].Name}이(가) 쓰러졌다... 패배했다.";
                _currentState = BattleState.BattleEnd;
            }
            else
            {
                _currentLog = "무엇을 할까?";
                _currentState = BattleState.SelectAction;
            }
        }
        else if (_currentState == BattleState.BattleEnd)
        {
            ReturnRequested?.Invoke();
        }
    }

    private void ExecuteEnemyTurn()
    {
        Random rand = new Random();
        int previousHp = player.Pokemons[0].CurrentHp;
        var skill = enemy.Pokemons[0].skills[rand.Next(0, 4)];
        int damage = enemy.Pokemons[0].AttackTo(skill, player.Pokemons[0]);
        player.Pokemons[0].TakeDamage(damage);

        _currentLog = $"적 {enemy.Pokemons[0].Name}의 {skill.Name}! {previousHp - player.Pokemons[0].CurrentHp}의 피해!";
        _currentState = BattleState.EnemyAttack;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        DrawGameObjects(buffer);

        DrawPokemonInfo(buffer, player.Pokemons[0], isPlayer: true);
        DrawPokemonInfo(buffer, enemy.Pokemons[0], isPlayer: false);

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

        var myMon = player.Pokemons[0];

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
                color = ConsoleColor.Yellow; // 선택된 항목은 노란색!
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
    CheckWin,       
    BattleEnd       
}