using System.Diagnostics;

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

    // Run the main game loop

    /// <summary>
    /// Run the main game loop
    /// </summary>
    public void RunGame()
    {
        // first we initalize the console
        InitalizeConsole();
        // we then show the start screen
        ShowStartScreen();
        // we then set the _gameStateIsRunning field to true
        _gameStateIsRunning = true;
        // set a target FPS
        const int targetFPS = 30;
        // set up a target "time" for our FPS to try to hit
        TimeSpan frameTime = TimeSpan.FromMilliseconds(1000.0 / targetFPS);
        // set up a stopwatch
        Stopwatch stopWatch = Stopwatch.StartNew();
        // get the previous "ticks"
        long getPreviousTicks = stopWatch.ElapsedMilliseconds;

        while (_gameStateIsRunning)
        {
            // get the current tick
            long currentTicks = stopWatch.ElapsedMilliseconds;
            // get the current milliseconds
            long elapedMilliseconds = currentTicks - getPreviousTicks;

            if (elapedMilliseconds < frameTime.TotalMilliseconds)
            {
                // set the current CPU thread to "sleep" for 1 millisecond
                Thread.Sleep(1);
                continue;
            }
            // set the previous ticks capture to the current captured ticks
            getPreviousTicks = currentTicks;
            // handle the user input
            HandleInput();
            // Update the ball position
            Update();
            // Render the game
            Render();
        }
    }



    // Helper methods, private scope, visible only inside this class

    /// <summary>
    /// Helper method used for initalizing the console
    /// </summary>
    private void InitalizeConsole()
    {
        // Clear the console feed.
        Console.Clear();

        // // we are with the try/catch block below, attempting to resize the terminal UI, not all terminals support this, so the try/catch can catch exceptions where resizing is not supported.
        // try
        // {
        //     if (Console.WindowWidth < _width || Console.WindowHeight < _height + 2)
        //     {
        //         Console.SetWindowSize(Math.Min(_width, Console.LargestWindowWidth), Math.Min(_height + 2, Console.LargestWindowHeight));
        //         Console.WriteLine($"Setting the height and width of the terminal is only supported on Windows, software is currently running on: {Environment.OSVersion} {Environment.MachineName}");
        //     }

        //     if (Console.BufferWidth < _width || Console.BufferHeight < _height + 2)
        //     {
        //         Console.SetBufferSize(Math.Max(_width, Console.BufferWidth), Math.Max(_height + 2, Console.BufferHeight));
        //         Console.WriteLine($"Setting the buffer height and width of the terminal is only supported Windows, software is currently running on: {Environment.OSVersion} {Environment.MachineName}");
        //     }
        // }
        // catch
        // {
        //     throw new InvalidProgramException("An error occured!");
        // }
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

        HandleWallCollision();
        HandlePaddleCollision();
        HandleScore();
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
        // left side paddle
        if (_ball!.X == _leftPaddle!.X + 1 && _ball!.Y >= _leftPaddle!.Y && _ball!.Y < _leftPaddle!.Y + _leftPaddle!.Height)
        {
            _ball!.X = _leftPaddle!.X + 1;
            _ball!.VelocityX = 1;
            _ball!.VelocityY = CalculateBounceDirection(_leftPaddle, _ball);
        }

        // right side paddle
        if (_ball!.X == _rightPaddle!.X - 1 && _ball!.Y >= _rightPaddle!.Y && _ball!.Y < _rightPaddle!.Y + _rightPaddle!.Height)
        {
            _ball!.X = _rightPaddle!.X - 1;
            _ball!.VelocityX = -1;
            _ball!.VelocityY = CalculateBounceDirection(_rightPaddle, _ball);
        }
    }

    /// <summary>
    /// Calculate the direction of the ball when it bounces from 1 racket to the other
    /// </summary>
    /// <param name="paddle">the paddle the ball hits</param>
    /// <param name="ball">the ball object</param>
    /// <returns></returns>
    private static int CalculateBounceDirection(Paddle paddle, Ball ball)
    {
        int paddleCenter = paddle.Y + paddle.Height / 2;
        int relativeImpact = ball.Y - paddleCenter;

        if (relativeImpact < 0)
        {
            return -1;
        }

        if (relativeImpact > 0)
        {
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Helper method that handles the current score for each player
    /// </summary>
    private void HandleScore()
    {
        if (_ball!.X <= 0)
        {
            _rightSideScore++;
        }

        else if (_ball!.X >= _width - 1)
        {
            _leftSideScore++;
        }
    }

    /// <summary>
    /// Helper method that resets the ball's X position
    /// </summary>
    /// <param name="ballDirectionX">current X position of the ball object</param>
    private void ResetBallPosition(int ballDirectionX)
    {
        _leftPaddle!.Reset(y: _height / 2 - _leftPaddle.Height / 2);
        _rightPaddle!.Reset(y: _height / 2 - _rightPaddle.Height / 2);

        _ball!.Reset(
            x: _width / 2,
            y: _height / 2,
            velocityX: ballDirectionX,
            velocityY: ballDirectionX > 0 ? 1 : -1
        );

        Render();
        Thread.Sleep(700);
    }

    private void Render()
    {
        _renderer!.RenderGame(
            leftSidePaddle: _leftPaddle!,
            rightSidePaddle: _rightPaddle!,
            ball: _ball!,
            leftSideScore: _leftSideScore,
            rightSideScore: _rightSideScore
        );
    }
}