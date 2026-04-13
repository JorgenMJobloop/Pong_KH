public sealed class MainGame
{
    // grid/board
    private readonly int _width;
    private readonly int _height;

    // "core" game fields
    private readonly Paddle? _leftPaddle;
    private readonly Paddle? _rightPaddle;
    private readonly Ball? _ball;
    private readonly Renderer? _renderer;
    private readonly InputHandler? _inputHandler;

    // score & game state
    private int _leftSideScore;
    private int _rightSideScore;
    private bool _gameStateIsRunning;

    public MainGame(int width, int height)
    {
        if (width < 40)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The width cannot be less than 40 pixels");
        }
        if (height < 15)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "The heigth cannot be lower 15 pixels of the terminal screen.");
        }

        _width = width;
        _height = height;

        // we can set the heigh & width of the paddles & the center bar
        int paddleHeight = 4;
        int centerY = height / 2 - paddleHeight / 2;

        _leftPaddle = new Paddle(x: 2, y: centerY, height: paddleHeight);
        _rightPaddle = new Paddle(x: width - 3, y: centerY, height: paddleHeight);

        _ball = new Ball(width / 2, height / 2, velocityX: -1, velocityY: -1);

        _renderer = new Renderer(width, height);
        _inputHandler = new InputHandler();
    }




    // Helper methods, private scope, visible only inside this class

    /// <summary>
    /// Helper method used for initalizing the console
    /// </summary>
    private void InitalizeConsole()
    {
        // Clear the console feed.
        Console.Clear();

        // we are with the try/catch block below, attempting to resize the terminal UI, not all terminals support this, so the try/catch can catch exceptions where resizing is not supported.
        try
        {
            if (Console.WindowWidth < _width || Console.WindowHeight < _height + 2)
            {
                Console.SetWindowSize(Math.Min(_width, Console.LargestWindowWidth), Math.Min(_height + 2, Console.LargestWindowHeight));
                Console.WriteLine($"Setting the height and width of the terminal is only supported on Windows, software is currently running on: {Environment.OSVersion} {Environment.MachineName}");
            }

            if (Console.BufferWidth < _width || Console.BufferHeight < _height + 2)
            {
                Console.SetBufferSize(Math.Max(_width, Console.BufferWidth), Math.Max(_height + 2, Console.BufferHeight));
                Console.WriteLine($"Setting the buffer height and width of the terminal is only supported Windows, software is currently running on: {Environment.OSVersion} {Environment.MachineName}");
            }
        }
        catch
        {
            throw new InvalidProgramException("An error occured!");
        }
    }

    /// <summary>
    /// Helper method that shows the start screen, this is hardcoded.
    /// </summary>
    private void ShowStartScreen()
    {
        Console.Clear();
        Console.SetCursorPosition(0, 2);
        Console.WriteLine("=================");
        Console.WriteLine("      PONG       ");
        Console.WriteLine("=================");
        Console.WriteLine(); // newline \n
        Console.WriteLine("Left paddle: W / S"); // Select side (default: left)
        Console.WriteLine("Right paddle: Key: UpArrow / Key: DownArrow");
        Console.WriteLine("Exit game: Escape\n");
        Console.WriteLine("Press any key to start the game...");

        Console.ReadKey(intercept: true);
    }

    /// <summary>
    /// Helper method that handles the incoming userinput
    /// </summary>
    private void HandleInput()
    {
        while (Console.KeyAvailable)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;

            switch (key)
            {
                case ConsoleKey.W:
                    _leftPaddle!.MoveUp(minimumYvalue: 1);
                    break;
                case ConsoleKey.S:
                    _leftPaddle!.MoveDown(maximumYvalue: _height - 2);
                    break;
                case ConsoleKey.UpArrow:
                    _rightPaddle!.MoveUp(minimumYvalue: 1);
                    break;
                case ConsoleKey.DownArrow:
                    _rightPaddle!.MoveDown(maximumYvalue: _height - 2);
                    break;
                // final check, escape key
                case ConsoleKey.Escape:
                    _gameStateIsRunning = false;
                    break;
            }
        }
    }

    /// <summary>
    ///  Update the ball's position
    /// </summary>
    private void Update()
    {
        _ball!.Move();

        // TODO: Handle collisions
    }

    /// <summary>
    /// Handle wall collision. When the ball hits the wall, a collision has occured and the opponent gains 1 point.
    /// </summary>
    private void HandleWallCollision()
    {
        if (_ball!.Y <= 1)
        {
            _ball!.Y = 1;
            _ball!.VelocityY *= -1;
        }
        else if (_ball!.Y >= _height - 2)
        {
            _ball!.Y = _height - 2;
            _ball!.VelocityY *= -1;
        }
    }

    /// <summary>
    /// Handle the paddle collision, if the paddle is outside the bounds of the game-grid, a collision MUST occur.
    /// </summary>
    private void HandlePaddleCollision()
    {

    }
}