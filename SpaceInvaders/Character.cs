using System;
using System.Linq;

namespace SpaceInvaders
{
    //Everything related to the controllable character
    static class Character
    {
        static string texture = "^";
        static public int lives = 3;
        static public int currentPos = 0;
        static public int lastPos = 0;
        static public int health = 100;
        static public string availablePowerUp = "None";
        static public int power = 50;

        //Displays the character on the screen
        static public void Draw(int dpos)
        {
            Console.SetCursorPosition(Program.WINDOW_WIDTH / 2 + dpos - 1, Program.WINDOW_HEIGHT - 3);
            Console.Write("  " + texture + " ");

            Console.SetCursorPosition(Program.WINDOW_WIDTH / 2 + dpos - 1, Program.WINDOW_HEIGHT - 2);
            Console.Write(" " + string.Concat(Enumerable.Repeat(texture, 3)) + " ");

            Console.SetCursorPosition(Program.WINDOW_WIDTH / 2 - 1 + dpos - 1, Program.WINDOW_HEIGHT - 1);
            Console.Write(" " + string.Concat(Enumerable.Repeat(texture, 5)) + " ");
        }

        //Changes character color depending on health then displays the character on screen at the current position
        static public void Update()
        {
            if (health == 100)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            if (health <= 70)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            if (health <= 50)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            if (health <= 30)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }

            Draw(currentPos);

            Console.ForegroundColor = ConsoleColor.White;
            lastPos = currentPos;
        }
    }
}