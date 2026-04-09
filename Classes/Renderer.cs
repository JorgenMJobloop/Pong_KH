using System.IO.Compression;
using System.Text;

public sealed class Renderer
{
    private readonly int _width;
    private readonly int _height;
    /// <summary>
    /// a readonly 2D char array buffer for rendering the grid
    /// </summary>
    private readonly char[,] _buffer;

    public Renderer(int width, int height)
    {
        _width = width;
        _height = height;
        _buffer = new char[height, width];
    }

    /// <summary>
    /// Wraps all the helper methods together into one main Game rendering method
    /// </summary>
    /// <param name="leftSidePaddle">player paddle on the left side</param>
    /// <param name="rightSidePaddle">player or cpu paddle on the right side</param>
    /// <param name="ball">game ball</param>
    /// <param name="rightSideScore">score for the player or cpu on the right side</param>
    /// <param name="leftSideScore">score for the player on the left side</param>
    public void RenderGame(Paddle leftSidePaddle, Paddle rightSidePaddle, Ball ball, int rightSideScore, int leftSideScore)
    {
        // we first call the ClearBuffer method
        ClearBuffer();
        // when the clearbuffer method has ran once, we call upon the DrawBorders method
        DrawBorders();
        // when the borders are drawn, we can draw the center line
        DrawCenterLine();
        // we can now draw the paddles
        DrawPaddle(leftSidePaddle);
        DrawPaddle(rightSidePaddle);
        // when the paddles are drawn on the terminal screen, we can draw the ball
        DrawBall(ball);
        // when the steps above have been ran, we draw the score for each side
        DrawGameScore(leftSideScore, rightSideScore);
        // when everything is drawn properly, we flush the buffer
        FlushBuffer();
    }

    // Helper methods for rendering graphics in the terminal.

    /// <summary>
    /// Clear the buffer on the terminal screen
    /// </summary>
    private void ClearBuffer()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _buffer[y, x] = ' ';
            }
        }
    }

    /// <summary>
    /// Draw the borders on the game grid
    /// </summary>
    private void DrawBorders()
    {
        // x-axis
        for (int x = 0; x < _width; x++)
        {
            _buffer[0, x] = '#';
            _buffer[_height - 1, x] = '#';
        }
        // y-axis
        for (int y = 0; y < _height; y++)
        {
            _buffer[y, 0] = '#';
            _buffer[y, _width - 1] = '#';
        }
    }

    /// <summary>
    /// Draw the center line in the game grid
    /// </summary>
    private void DrawCenterLine()
    {
        int centerX = _width / 2;

        for (int y = 1; y < _height - 1; y++)
        {
            if (y % 2 == 0)
            {
                _buffer[y, centerX] = '|';
            }
        }
    }

    /// <summary>
    /// Draw the paddle in the game grid
    /// </summary>
    /// <param name="paddle">the incoming Paddle object</param>
    private void DrawPaddle(Paddle paddle)
    {
        for (int i = 0; i < paddle.Height; i++)
        {
            // the paddles y postion (paddle.Y + i)
            int y = paddle.Y + i;

            // do the rendering on the line below
            if (y > 0 && y < _height - 1)
            {
                _buffer[y, paddle.X] = '█';
            }
        }
    }

    /// <summary>
    /// Draw the ball on the game grid
    /// </summary>
    /// <param name="ball">object instance of the Ball class</param>
    private void DrawBall(Ball ball)
    {
        if (ball.X > 0 && ball.X < _width - 1 && ball.Y > 0 && ball.Y < _height - 1)
        {
            _buffer[ball.Y, ball.X] = 'O';
        }
    }

    /// <summary>
    /// Draw the game score for both right and left side on the terminal screen
    /// </summary>
    /// <param name="leftSideScore">player on left sides score</param>
    /// <param name="rightSideScore">player on right sides score</param>
    private void DrawGameScore(int leftSideScore, int rightSideScore)
    {
        string scoreText = $"{leftSideScore} : {rightSideScore}";
        // place the score text in a reasonable position relative to the main game rendering
        int startXposition = (_width - scoreText.Length) / 2;

        for (int i = 0; i < scoreText.Length; i++)
        {
            _buffer[0, startXposition + i] = scoreText[i];
        }
    }

    /// <summary>
    /// Flush the buffer
    /// </summary>
    private void FlushBuffer()
    {
        StringBuilder sb = new StringBuilder(_height * (_width + Environment.NewLine.Length));


        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                sb.Append(_buffer[y, x]);
            }

            if (y < _height - 1)
            {
                sb.AppendLine();
            }
        }

        Console.SetCursorPosition(0, 0);
        Console.Write(sb.ToString());
    }
}