using Framework.Engine;

public class Player : Trainer
{
    public Player(Scene scene, int startX, int startY, string name) : base(scene, startX, startY, name)
    {
        _name = name;
        _position.X = startX;
        _position.Y = startY;
        isBattle = false;
    }

    private void Move()
    {
        if (Input.IsKey(ConsoleKey.UpArrow))
        {
            _position.Y = _position.Y - 1;
            if (_position.Y < 1)
            {
                _position.Y = 1;
            }
        }
        else if (Input.IsKey(ConsoleKey.DownArrow))
        {
            _position.Y = _position.Y + 1;
            if (_position.Y > 28)
            {
                _position.Y = 28;
            }
        }
        else if (Input.IsKey(ConsoleKey.LeftArrow))
        {
            _position.X = _position.X - 1;
            if (_position.X < 1)
            {
                _position.X = 1;
            }
        }
        else if (Input.IsKey(ConsoleKey.RightArrow))
        {
            _position.X = _position.X + 1;
            if (_position.X > 58)
            {
                _position.X = 58;
            }
        }
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (isBattle)
        {
            // 처음에는 첫 번째 포켓몬을 출력하도록 수정
            buffer.WriteText(2, 13, " /\\_/\\");
            buffer.WriteText(2, 14, "( o.o )");
            buffer.WriteText(2, 15, " > ^ <");
            buffer.WriteText(2, 16, $"[{pokemons[0].Name}] {pokemons[0].CurrentHp}/{pokemons[0].MaxHp}", ConsoleColor.Blue);
        }
        else
        {
            buffer.SetCell(Position.X, Position.Y, '@', ConsoleColor.Green);
        }
    }

    public override void Update(float deltaTime)
    {
        Move();
    }
}