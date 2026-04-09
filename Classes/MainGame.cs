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
}