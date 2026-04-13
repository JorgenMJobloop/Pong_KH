public sealed class Ball
{
    /// <summary>
    /// X value of the ball, can be "gotten/fetch" anywhere and it's value can be set everywhere this class is instanciated as an object
    /// </summary>
    public int X { get; set; }
    public int Y { get; set; }
    // physics properties
    public int VelocityX { get; set; }
    public int VelocityY { get; set; }

    public Ball(int x, int y, int velocityX, int velocityY)
    {
        X = x;
        Y = y;
        VelocityX = velocityX;
        VelocityY = velocityY;
    }

    // the methods below, handles the ball's physics

    public void Move()
    {
        X += VelocityX;
        Y += VelocityY;
    }

    public void Reset(int x, int y, int velocityX, int velocityY)
    {
        X = x;
        Y = y;
        VelocityX = velocityX;
        VelocityY = velocityY;
    }
}