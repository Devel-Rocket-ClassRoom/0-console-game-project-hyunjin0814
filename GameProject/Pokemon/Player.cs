using Framework.Engine;

public class Player : Trainer
{
    private (int X, int Y) _previousPosition;

    public (int X, int Y) PreviousPosition => _previousPosition;
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

    public void PositionSave()
    {
        _previousPosition = Position;
    }

    public void PositionLoad()
    {
        _position = PreviousPosition;
    }

    public override void Draw(ScreenBuffer buffer)
    {
        if (isBattle)
        {
            return;
        }
        buffer.SetCell(Position.X, Position.Y, '@', ConsoleColor.Green);
    }

    public override void Update(float deltaTime)
    {
        Move();
    }
}