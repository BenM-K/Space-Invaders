using System;
using System.Linq;

namespace SpaceInvaders
{
    //Everything related to the aliens (enemies)
    class Alien
    {
        public int Health { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        const int spawnBoundsLeft = 34;
        const int spawnBoundsRight = 124;
        public static int speedDelay = 25;
        public static int spawnSpeedDelay = 150;
        static int speedTimer = 0;
        static int spawnTimer = 0;
        public static int numPerWave = 5;
        public static int remaining = numPerWave;
        public static string texture = "V";
        static Random random = new Random();

        public Alien(int health, int x, int y)
        {
            Health = health;
            X = x;
            Y = y;
        }

        //Create a new alien object and add it to the list of aliens
        static public void CreateAlien()
        {
            if (Game.firstRun)
            {
                Game.firstRun = false;

                int X = random.Next(spawnBoundsLeft, spawnBoundsRight);

                Game.aliens.Add(new Alien(100, X, 0));

                Console.SetCursorPosition(X, 0);
                Console.Write(texture);
            }
            else
            {
                if (Game.aliens.Count != Alien.remaining)
                {
                    spawnTimer++;

                    if (spawnTimer % spawnSpeedDelay == 0)
                    {
                        int X = random.Next(spawnBoundsLeft, spawnBoundsRight);

                        Game.aliens.Add(new Alien(100, X, 0));

                        Console.SetCursorPosition(X, 0);
                        Console.Write(texture);
                    }
                }
            }
        }

        //Display the aliens on the screen
        static public void Draw()
        {
            speedTimer++;

            if (Game.aliens.Count > 0)
            {
                foreach (Alien alien in Game.aliens.ToList())
                {
                    if (!(alien.Y == Program.WINDOW_BOTTOM))
                    {
                        if (speedTimer % speedDelay == 0)
                        {
                            if (Bullet.bX != 0 && Bullet.bY != 0)
                            {
                                Console.SetCursorPosition(Bullet.bX, Bullet.bY);
                                Console.Write(Game.backgroundTexture);
                                Bullet.bX = 0;
                                Bullet.bY = 0;
                            }
                            Console.SetCursorPosition(alien.X, alien.Y);
                            Console.Write(Game.backgroundTexture);
                            alien.Y++;
                            Console.SetCursorPosition(alien.X, alien.Y);
                            if (alien.Health == 50)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                            Console.Write(texture);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else
                    {
                        Console.SetCursorPosition(alien.X, alien.Y);
                        Console.Write(Game.backgroundTexture);
                        Game.aliens.Remove(alien);
                        remaining--;
                        Character.health -= 10;
                        if (!Program.audioMuted)
                        {
                            Game.Effects.Open(new Uri(Files.savePath + "\\audio\\SpaceInvaders.audio.dmgtaken.mp3"));
                            Game.Effects.Play();
                        }
                    }
                }
            }
        }
    }
}