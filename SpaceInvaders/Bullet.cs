using System;
using System.Linq;
using System.Threading;

namespace SpaceInvaders
{
    //Everything related to the bullets shot by the controllable character
   class Bullet
    {
        string Texture { get; set; }
        int X { get; set; }
        int Y { get; set; }

        //Delay used to prevent the player from spamming the shoot button
        public static Boolean cooldownDone = true;
        static int shootCoolDownDelay = 250;
        static public int bX = 0;
        static public int bY = 0;

        public Bullet(string texture, int x, int y)
        {
            Texture = texture;
            X = x;
            Y = y;
        }

        //Display the bullet(s) on the screen
        static public void Draw()
        {
            if (Game.bullets.Count > 0)
            {
                foreach (Bullet bullet in Game.bullets.ToList())
                {
                    if (bullet.Y > Console.WindowTop)
                    {
                        Console.SetCursorPosition(bullet.X, bullet.Y - 1);
                        Console.Write(bullet.Texture);
                        Console.SetCursorPosition(bullet.X, bullet.Y);
                        Console.Write(Game.backgroundTexture);
                        bullet.Y += -1;
                        DetectHit(bullet.X, bullet.Y);
                    }
                    else
                    {
                        Game.bullets.Remove(bullet);
                        Console.SetCursorPosition(bullet.X, bullet.Y);
                        Console.Write(Game.backgroundTexture);
                    }
                }
            }
        }

        //Detect if a bullet hits an enemy
        static void DetectHit(int bulletX, int bulletY)
        {
            foreach (Alien alien in Game.aliens.ToList())
            {
                if (bulletX == alien.X)
                {
                    if ((bulletY == alien.Y) || (bulletY == alien.Y + 1))
                    {
                        Bullet bullet = Game.bullets.FirstOrDefault(p => p.X == bulletX && p.Y == bulletY);
                        Game.bullets.Remove(bullet);
                        alien.Health -= Character.power;
                        Game.Effects.Open(new Uri(Files.savePath + "\\audio\\SpaceInvaders.audio.hit.mp3"));
                        Game.Effects.Play();
                        Console.SetCursorPosition(alien.X, alien.Y + 1);
                        Console.Write(Game.backgroundTexture);
                        Console.SetCursorPosition(alien.X, alien.Y);
                        Console.Write(Game.backgroundTexture);
                        if (alien.Health != 0)
                        {
                            Console.SetCursorPosition(alien.X, alien.Y);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(Alien.texture);
                            Console.ForegroundColor = ConsoleColor.White;
                            bX = alien.X;
                            bY = alien.Y;
                        }
                    }

                    if (alien.Health == 0)
                    {
                        Alien.remaining--;
                        Game.aliens.Remove(alien);

                        if (Game.doublePoints)
                        {
                            Game.score += Game.pointsPerKill * 2;
                            //Console.SetCursorPosition(x + 1, y);
                            //Console.Write("+" + Game.pointsPerKill * 2);
                        }
                        else
                        {
                            Game.score += Game.pointsPerKill;
                            //Console.SetCursorPosition(x + 1, y);
                            //Console.Write("+" + Game.pointsPerKill);
                        }
                        if (!Program.audioMuted)
                        {
                            Game.Effects.Open(new Uri(Files.savePath + "\\audio\\SpaceInvaders.audio.hit.mp3"));
                            Game.Effects.Play();
                        }
                    }
                }
            }
        }

        //Delay used to prevent the player from spamming the shoot button
        static public void StartCoolDown()
        {
            Thread.Sleep(shootCoolDownDelay);
            cooldownDone = true;
        }
    }
}