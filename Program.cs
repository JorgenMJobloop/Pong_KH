using System.Text;

namespace Pong;

class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;

        // try/catch when trying to run the game
        try
        {
            var game = new MainGame(width: 80, height: 24);
            game.RunGame();
        }
        finally
        {
            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }
}
