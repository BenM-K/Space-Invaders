using System;
using System.Threading;
using System.Collections.Generic;

namespace SpaceInvaders
{
    //Everything related to the actual game
    class Game
    {
        //Setting the character movement boundaries
        public const int LEFT_BOUNDS = -47;
        public const int RIGHT_BOUNDS = 43;

        //List to hold the bullets (objects) on screen
        public static List<Bullet> bullets = new List<Bullet>();

        //List to hold the aliens (objects) on screen
        public static List<Alien> aliens = new List<Alien>();

        //Create sound player to play sound effects
        public static System.Windows.Media.MediaPlayer Effects = new System.Windows.Media.MediaPlayer();

        static Boolean gameOver = false;
        public static Boolean firstRun = true;
        public static Boolean doublePoints = false;
        public static string backgroundTexture = " ";
        public static string verticalBorderTexture = "|";
        public static string horizontalBorderTexture = "-";
        public static int pointsPerKill = 10;
        public static int score = 0;
        static int wave = 1;
        static int hiScore = 0;
        static int speed = 15;
        static int gameX = Program.WINDOW_WIDTH / 2;
        static int gameY = Program.WINDOW_HEIGHT / 2 - 5;

        //Initialize the game
        public static void StartGame()
        {
            Program.musicPlaying = false;

            Console.Title = "";

            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();

            DisplayGameControlsText();

            if (!Program.audioMuted)
            {
                Effects.Open(new Uri(Files.savePath + "\\audio\\SpaceInvaders.audio.countdown.mp3"));
                Effects.Play();
            }

            Console.SetCursorPosition(gameX, gameY);
            Console.Write("3..");

            Thread.Sleep(1000);

            Console.SetCursorPosition(gameX, gameY);
            Console.Write("2..");

            Thread.Sleep(1000);

            Console.SetCursorPosition(gameX, gameY);
            Console.Write("1..");

            Thread.Sleep(1000);

            Console.SetCursorPosition(gameX, gameY);
            Console.Write("GO!");

            Thread.Sleep(500);

            Program.DisableInput();

            Console.Clear();

            GameLoop();
        }

        //Main game loop
        static void GameLoop()
        {
            Character.Update();

            DisplayBorders();

            DisplayGameInfo();

            Console.SetCursorPosition(gameX - 28, gameY);
            Console.Write("                                                           ");

            Console.SetCursorPosition(gameX - 1, gameY);
            Console.Write("WAVE " + Game.wave);

            Thread.Sleep(1500);

            Console.SetCursorPosition(gameX - 1, gameY);
            Console.Write("       ");

            if (Program.audioMuted)
            {
                Console.Title = "Space Invaders | In Game (Wave " + Game.wave + ", Sound: OFF)";
                Program.changeSong = true;
            }
            else
            {
                Console.Title = "Space Invaders | In Game (Wave " + Game.wave + ", Sound: ON)";
                Program.changeSong = true;
                Program.musicPlaying = true;
            }

            Program.DisableInput();

            while (!gameOver)
            {
                GameDetectKey();

                Character.Update();

                Alien.CreateAlien();

                Alien.Draw();

                Bullet.Draw();

                DisplayGameInfo();

                if (Character.health <= 0)
                {
                    Character.lives -= 1;
                    Character.health = 100;

                    if (Character.lives <= 0)
                    {
                        gameOver = true;
                    }
                    else
                    {
                        if (!Program.audioMuted)
                        {
                            Effects.Open(new Uri(Files.savePath + "\\audio\\SpaceInvaders.audio.lifelost.mp3"));
                            Effects.Play();
                        }
                    }
                }

                if (Alien.remaining == 0 && Character.lives > 0)
                {
                    Program.musicPlaying = false;
                    Console.SetCursorPosition(gameX - 28, gameY);
                    Console.Write("Good job, but there's still more alien invaders to fight...");
                    Thread.Sleep(1500);
                    wave++;
                    if (Program.audioMuted)
                    {
                        Console.Title = "Space Invaders | In Game (Wave " + Game.wave + ", Sound: OFF)";
                    }
                    else
                    {
                        Console.Title = "Space Invaders | In Game (Wave " + Game.wave + ", Sound: ON)";
                    }
                    Alien.numPerWave += 5;
                    Alien.remaining = Alien.numPerWave;
                    Alien.spawnSpeedDelay -= 5;
                    Alien.speedDelay -= 2;
                    firstRun = true;
                    GameLoop();
                }

                Thread.Sleep(speed);
            }

            GameOver();
        }

        static void GameOver()
        {
            Environment.Exit(0);

            Console.Clear();

            if (!Program.audioMuted)
            {
                //playgameover sound
            }

            //display stats etc
        }

        static void GameDetectKey()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo dKeyboard = Console.ReadKey(true);

                if (dKeyboard.Key == ConsoleKey.LeftArrow)
                {
                    if (!(Character.currentPos <= LEFT_BOUNDS))
                    {
                        Character.lastPos = Character.currentPos;
                        Character.currentPos--;
                    }
                }
                else if (dKeyboard.Key == ConsoleKey.RightArrow)
                {
                    if (!(Character.currentPos >= RIGHT_BOUNDS))
                    {
                        Character.lastPos = Character.currentPos;
                        Character.currentPos++;
                    }
                }
                else if (dKeyboard.Key == ConsoleKey.Spacebar || dKeyboard.Key == ConsoleKey.UpArrow)
                {
                    if (Bullet.cooldownDone)
                    {
                        bullets.Add(new Bullet("|", Program.WINDOW_WIDTH / 2 + Character.currentPos + 1, Program.WINDOW_HEIGHT - 4));

                        if (!Program.audioMuted)
                        {
                            Effects.Open(new Uri(Files.savePath + "\\audio\\SpaceInvaders.audio.shoot.mp3"));
                            Effects.Play();
                        }

                        Bullet.cooldownDone = false;
                        new Thread(() => Bullet.StartCoolDown()).Start();
                    }
                }
                else if (dKeyboard.Key == ConsoleKey.M)
                {
                    if (!Program.audioMuted)
                    {
                        Console.Title = "Space Invaders | In Game (Wave " + Game.wave + ", Sound: OFF)";
                        Program.audioMuted = true;
                        Program.musicPlaying = false;
                    }
                    else
                    {
                        Console.Title = "Space Invaders | In Game (Wave " + Game.wave + ", Sound: ON)";
                        Program.audioMuted = false;
                        Program.musicPlaying = true;
                    }
                }
                else if (dKeyboard.Key == ConsoleKey.N)
                {
                    if (!Program.changeSong)
                    {
                        Program.changeSong = true;
                    }
                    else
                    {
                        Program.changeSong = false;
                    }
                }
                else if (dKeyboard.Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }
                else if (dKeyboard.Key == ConsoleKey.DownArrow)
                {
                    //activate powerup (if there is one)
                }
            }
        }

        static void DisplayGameInfo()
        {
            Console.SetCursorPosition(3, 7);
            Console.Write("High Score: " + hiScore);

            Console.SetCursorPosition(3, 12);
            Console.Write("Score: " + score);

            Console.SetCursorPosition(3, 17);
            Console.Write("Aliens Remaining: " + Alien.remaining + " ");

            Console.SetCursorPosition(3, 22);
            Console.Write("Available Power-up: " + Character.availablePowerUp);

            Console.SetCursorPosition(3, 27);
            Console.Write("Health: " + Character.health + " ");

            Console.SetCursorPosition(3, 32);
            Console.Write("Lives: " + Character.lives);

            Console.SetCursorPosition(3, 37);
            Console.Write("Wave: " + wave);

            Console.SetCursorPosition(131, 7);
            Console.Write("Move: Left/Right Arrow Keys");

            Console.SetCursorPosition(131, 12);
            Console.Write("Shoot: Spacebar/Up Arrow");

            Console.SetCursorPosition(131, 17);
            Console.Write("Activate Powerup: Down Arrow");

            Console.SetCursorPosition(131, 22);
            Console.Write("Quit Game: Escape");

            Console.SetCursorPosition(131, 27);
            Console.Write("Toggle Mute: M");

            Console.SetCursorPosition(131, 32);
            Console.Write("Change Song: N");
        }

        static void DisplayGameControlsText()
        {
            Console.SetCursorPosition(gameX - 12, gameY + 5);
            Console.Write("Move: Left/Right Arrow Keys");

            Console.SetCursorPosition(gameX - 12, gameY + 8);
            Console.Write("Shoot: Spacebar/Up Arrow");

            Console.SetCursorPosition(gameX - 12, gameY + 11);
            Console.Write("Activate Powerup: Down Arrow");

            Console.SetCursorPosition(gameX - 12, gameY + 14);
            Console.Write("Quit Game: Escape");

            Console.SetCursorPosition(gameX - 12, gameY + 17);
            Console.Write("Toggle Mute: M");

            Console.SetCursorPosition(gameX - 12, gameY + 20);
            Console.Write("Change Song: N");

            for (int i = 0; i < 20; i++)
            {
                Console.SetCursorPosition(gameX - 16, gameY + 3 + i);
                Console.Write(verticalBorderTexture);
            }

            for (int i = 0; i < 20; i++)
            {
                Console.SetCursorPosition(gameX + 18, gameY + 3 + i);
                Console.Write(verticalBorderTexture);
            }

            for (int i = 0; i < 33; i++)
            {
                Console.SetCursorPosition(gameX - 15 + i, gameY + 3);
                Console.Write(horizontalBorderTexture);
            }

            for (int i = 0; i < 33; i++)
            {
                Console.SetCursorPosition(gameX - 15 + i, gameY + 22);
                Console.Write(horizontalBorderTexture);
            }

            Program.DisableInput();

            while (!Console.KeyAvailable)
            {
                Console.SetCursorPosition(gameX - 12, gameY);
                Console.Write("Press any key when ready...");
            }

            Console.SetCursorPosition(gameX - 12, gameY);
            Console.Write("                           ");
        }

        static void DisplayBorders()
        {
            for (int i = 0; i < Program.WINDOW_HEIGHT; i++)
            {
                Console.SetCursorPosition(30, Console.WindowTop + i);
                Console.Write(verticalBorderTexture);

                Console.SetCursorPosition(128, Console.WindowTop + i);
                Console.Write(verticalBorderTexture);
            }
        }
    }
}