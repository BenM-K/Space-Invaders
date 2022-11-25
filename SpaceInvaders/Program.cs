using System;
using System.IO;
using System.Threading;

namespace SpaceInvaders
{
    //add comments
    //add settings file (difficulty, sound, mute, etc)
    //make keyboard movement simultaneous
    //test if all disableinputs needed
    //add powerups
    //prevent window resize?? if not add dont resize window to splash screen
    //at game over save shots fired, enemies killed, wave number, score, save to file

    //Task.Run(() => SetWindowSize());

    //Main program class
    class Program
    {
        //Setting width and height of the console window
        public const int WINDOW_WIDTH = 160;
        public const int WINDOW_HEIGHT = 50;
        public const int WINDOW_BOTTOM = 49;

        //Create thread to play background music
        static Thread musicThread = new Thread(new ThreadStart(PlayRandomSong));

        //Set how fast the text displays
        static int textTypeSpeed = 14;

        //Used to make sure the same song doesn't play twice
        static int lastSong = -1;

        //Makes sure user can only input keys when menu text is finished displaying
        public static Boolean acceptInput = false;

        //Sets how fast the main menu logo will scroll through random colors
        static int logoColorScrollSpeed = 80;

        public static string[] songNames = new string[] { "SpaceInvaders.audio.music1", "SpaceInvaders.audio.music2", "SpaceInvaders.audio.music3", "SpaceInvaders.audio.music4", "SpaceInvaders.audio.music5",
                                                   "SpaceInvaders.audio.music6", "SpaceInvaders.audio.music7", "SpaceInvaders.audio.music8", "SpaceInvaders.audio.music9", "SpaceInvaders.audio.music10" };
        static string[] logoLines = new string[] {"  ██████  ██▓███   ▄▄▄       ▄████▄  ▓█████     ██▓ ███▄    █ ██▒   █▓ ▄▄▄      ▓█████▄ ▓█████  ██▀███    ██████ ",
                                                  "▒██    ▒ ▓██░  ██▒▒████▄    ▒██▀ ▀█  ▓█   ▀    ▓██▒ ██ ▀█   █▓██░   █▒▒████▄    ▒██▀ ██▌▓█   ▀ ▓██ ▒ ██▒▒██    ▒ ",
                                                  "░ ▓██▄   ▓██░ ██▓▒▒██  ▀█▄  ▒▓█    ▄ ▒███      ▒██▒▓██  ▀█ ██▒▓██  █▒░▒██  ▀█▄  ░██   █▌▒███   ▓██ ░▄█ ▒░ ▓██▄   ",
                                                  "  ▒   ██▒▒██▄█▓▒ ▒░██▄▄▄▄██ ▒▓▓▄ ▄██▒▒▓█  ▄    ░██░▓██▒  ▐▌██▒ ▒██ █░░░██▄▄▄▄██ ░▓█▄   ▌▒▓█  ▄ ▒██▀▀█▄    ▒   ██▒",
                                                  "▒██████▒▒▒██▒ ░  ░ ▓█   ▓██▒▒ ▓███▀ ░░▒████▒   ░██░▒██░   ▓██░  ▒▀█░   ▓█   ▓██▒░▒████▓ ░▒████▒░██▓ ▒██▒▒██████▒▒",
                                                  "▒ ▒▓▒ ▒ ░▒▓▒░ ░  ░ ▒▒   ▓▒█░░ ░▒ ▒  ░░░ ▒░ ░   ░▓  ░ ▒░   ▒ ▒   ░ ▐░   ▒▒   ▓▒█░ ▒▒▓  ▒ ░░ ▒░ ░░ ▒▓ ░▒▓░▒ ▒▓▒ ▒ ░",
                                                  "░ ░▒  ░ ░░▒ ░       ▒   ▒▒ ░  ░  ▒    ░ ░  ░    ▒ ░░ ░░   ░ ▒░  ░ ░░    ▒   ▒▒ ░ ░ ▒  ▒  ░ ░  ░  ░▒ ░ ▒░░ ░▒  ░ ░",
                                                  "░  ░  ░  ░░         ░   ▒   ░           ░       ▒ ░   ░   ░ ░     ░░    ░   ▒    ░ ░  ░    ░     ░░   ░ ░  ░  ░  ",
                                                  "      ░                 ░  ░░ ░         ░  ░    ░           ░      ░        ░  ░   ░       ░  ░   ░           ░  ",
                                                  "                            ░                                     ░              ░                               "};
        static int menuX = 25;
        static int menuY = Program.WINDOW_HEIGHT / 2 - 5;
        static int logoY = Program.WINDOW_HEIGHT / 2 - 22;
        public static Boolean audioMuted = false;
        public static Boolean musicPlaying = true;
        public static Boolean changeSong = false;
        public static Random random = new Random();

        static void Main(string[] args)
        {
            //Sets up the console window (hides cursor, sets window size, creates save directory, etc)
            Initialize();

            //Game.StartGame();

            //Saves all game audio to the disk
            Files.SaveAudioToDisk();

            //Blocks user input by processing every keystroke without displaying it on screen
            DisableInput();

            Console.Clear();

            //Show the startup screen
            DisplaySplashScreen();

            Console.Clear();

            //Start thread to play random music in the background
            musicThread.Start();

            //Display the main menu
            DisplayMainMenuFirstRun();
        }

        static void Initialize()
        {
            Console.CursorVisible = false;
            Console.WindowHeight = WINDOW_HEIGHT;
            Console.WindowWidth = WINDOW_WIDTH;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = "";
            Directory.CreateDirectory(Files.savePath + "\\audio\\");
        }

        public static void DisableInput()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }

        //Display text on the screen one character at a time to create a scroll effect
        public static void TypeText(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(text[i]);
                Thread.Sleep(textTypeSpeed);
            }
        }

        static void DisplaySplashScreen()
        {
            int X = Program.WINDOW_WIDTH / 2 - 10;
            int Y = Program.WINDOW_HEIGHT / 2 - 23;

            Console.Title = "Space Invaders | Welcome!";

            Console.SetCursorPosition(X, Y);
            Console.Write("Welcome to Space Invaders!");

            Console.SetCursorPosition(X - 68, Y + 3);
            Console.Write("- The goal of the game is simple: Move your character left or right to aim, then shoot the aliens before they reach the bottom of the screen.");

            Console.SetCursorPosition(X - 68, Y + 6);
            Console.Write("- Aliens take 2 hits to kill (they have 100 health, one bullet does 50 damage.)");

            Console.SetCursorPosition(X - 68, Y + 9);
            Console.Write("- You start with 100 health and 3 lives. For every alien that reaches the bottom of the screen, you lose 10 health.");

            Console.SetCursorPosition(X - 68, Y + 12);
            Console.Write("- The color of your character changes depending on your health. Once you reach 0 health, you lose a life, and once you reach 0 lives, it's game over.");

            Console.SetCursorPosition(X - 68, Y + 15);
            Console.Write("- The game plays out in infinite waves. Every wave has a certain amount of aliens you need to kill in order to progress to the next one.");

            Console.SetCursorPosition(X - 68, Y + 18);
            Console.Write("- As the waves progress, more aliens spawn and they come down faster.");

            Console.SetCursorPosition(X - 68, Y + 21);
            Console.Write("- Mystery power-ups will randomly spawn that can grant the player abilities they can activate, such as temporary invincibility, double points, insta-kill,        random healing and bonus lives.");

            Console.SetCursorPosition(X - 68, Y + 25);
            Console.Write("- The power-ups can also be bad for the player, doing things like temporarily causing aliens to deal more damage, randomly taking away health/lives, and          instantly causing a game over (if you're really unlucky...)");

            Console.SetCursorPosition(X, Y + 28);
            Console.Write("Move: Left/Right Arrow Keys");

            Console.SetCursorPosition(X, Y + 30);
            Console.Write("Shoot: Spacebar/Up Arrow");

            Console.SetCursorPosition(X, Y + 32);
            Console.Write("Activate Powerup: Down Arrow");

            Console.SetCursorPosition(X, Y + 34);
            Console.Write("Quit Game: Escape");

            Console.SetCursorPosition(X, Y + 36);
            Console.Write("Toggle Mute: M");

            Console.SetCursorPosition(X, Y + 38);
            Console.Write("Change Song: N");

            Console.SetCursorPosition(X - 15, Y + 41);
            Console.Write("That's all there is to it, see how far you can go!");

            Y = Program.WINDOW_HEIGHT / 2 + 21;

            Console.SetCursorPosition(X, Y);
            Console.Write("Press any key to begin...");

            Console.SetCursorPosition(X, Y + 2);
            Console.Write("(make sure sound is on)");
            do
            {
                Console.SetCursorPosition(X + 25, Y);

                for (int x = 1; x < 5; x++)
                {
                    Thread.Sleep(500);
                    Console.Write("\b ");
                    Console.SetCursorPosition(X + 25 - x, Y);

                    if (Console.KeyAvailable)
                    {
                        acceptInput = false;

                        //Process keystroke without displaying it on screen
                        Console.ReadKey(true);

                        //Return to main to run music and display the main menu
                        return;
                    }
                }

                Console.SetCursorPosition(X, Y);
                Console.Write("Press any key to begin...");
            }
            while (!Console.KeyAvailable);
        }

        static void DisplayInstructions()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.White;

            int X = Program.WINDOW_WIDTH / 2 - 9;
            int Y = Program.WINDOW_HEIGHT / 2 - 23;

            if (audioMuted)
            {
                Console.Title = "Space Invaders | Instructions (Sound: OFF)";
            }
            else
            {
                Console.Title = "Space Invaders | Instructions (Sound: ON)";
            }

            Console.SetCursorPosition(X - 68, Y);
            Console.Write("- The goal of the game is simple: Move your character left or right to aim, then shoot the aliens before they reach the bottom of the screen.");

            Console.SetCursorPosition(X - 68, Y + 3);
            Console.Write("- Aliens take 2 hits to kill (they have 100 health, one bullet does 50 damage.)");

            Console.SetCursorPosition(X - 68, Y + 6);
            Console.Write("- You start with 100 health and 3 lives. For every alien that reaches the bottom of the screen, you lose 10 health.");

            Console.SetCursorPosition(X - 68, Y + 9);
            Console.Write("- The color of your character changes depending on your health. Once you reach 0 health, you lose a life, and once you reach 0 lives, it's game over.");

            Console.SetCursorPosition(X - 68, Y + 12);
            Console.Write("- The game plays out in infinite waves. Every wave has a certain amount of aliens you need to kill in order to progress to the next one.");

            Console.SetCursorPosition(X - 68, Y + 15);
            Console.Write("- As the waves progress, more aliens spawn and they come down faster.");

            Console.SetCursorPosition(X - 68, Y + 18);
            Console.Write("- Mystery power-ups will randomly spawn that can grant the player abilities they can activate, such as temporary invincibility, double points, insta-kill,        random healing and bonus lives.");

            Console.SetCursorPosition(X - 68, Y + 22);
            Console.Write("- The power-ups can also be bad for the player, doing things like temporarily causing aliens to deal more damage, randomly taking away health/lives, and          instantly causing a game over (if you're really unlucky...)");

            Console.SetCursorPosition(X, Y + 28);
            Console.Write("Move: Left/Right Arrow Keys");

            Console.SetCursorPosition(X, Y + 30);
            Console.Write("Shoot: Spacebar/Up Arrow");

            Console.SetCursorPosition(X, Y + 32);
            Console.Write("Activate Powerup: Down Arrow");

            Console.SetCursorPosition(X, Y + 34);
            Console.Write("Quit Game: Escape");

            Console.SetCursorPosition(X, Y + 36);
            Console.Write("Toggle Mute: M");

            Console.SetCursorPosition(X, Y + 38);
            Console.Write("Change Song: N");

            Y = Program.WINDOW_HEIGHT / 2 + 21;

            Console.SetCursorPosition(X - 8, Y);
            Console.Write("Press any key to return to main menu...");

            do
            {
                Console.SetCursorPosition(X + 31, Y);

                for (int x = 1; x < 4; x++)
                {
                    Thread.Sleep(500);
                    Console.Write("\b ");
                    Console.SetCursorPosition(X + 31 - x, Y);

                    if (Console.KeyAvailable)
                    {
                        //Process keystroke without displaying it on screen
                        Console.ReadKey(true);

                        //Return to main to run music and display the main menu
                        Console.Clear();
                        DisplayMainMenu();
                    }
                }

                Thread.Sleep(500);
                Console.SetCursorPosition(X - 8, Y);
                Console.Write("Press any key to return to main menu...");
            }
            while (!Console.KeyAvailable);

            Console.Clear();
            DisplayMainMenu();
        }

        static void PlayRandomSong()
        {
            //Create mediaplayer object to play music
            var Music = new System.Windows.Media.MediaPlayer();

            //Get a random integer to be used to access a song in the songNames array
            int songName = random.Next(0, songNames.Length);

            //Checks if the random song choosen is the same as the one that was just played
            while (songName == Program.lastSong)
            {
                songName = random.Next(0, songNames.Length);
            }

            //Load the song from the file path and play it
            Music.Open(new Uri(Files.savePath + "\\audio\\" + songNames[songName] + ".mp3"));
            Music.Play();

            Program.lastSong = songName;

            //Current position of the song
            string currentPos = Music.Position.ToString();

            //Total duration of the song
            string songDuration = Music.NaturalDuration.ToString();

            //While the song isn't over
            while (currentPos != songDuration)
            {
                if (audioMuted || !musicPlaying)
                {
                    Music.Volume = 0;
                }
                else if (Music.Volume == 0)
                {
                    Music.Volume = 0.5;
                }

                if (changeSong)
                {
                    changeSong = false;
                    Music.Stop();
                    PlayRandomSong();
                }

                currentPos = Music.Position.ToString();
                songDuration = Music.NaturalDuration.ToString();
            }

            PlayRandomSong();
        }

        public static void DisplayMainMenuFirstRun()
        {
            ConsoleColor lastColor;

            Program.acceptInput = false;

            Console.Title = "Space Invaders | Main Menu (Sound: ON)";

            Console.ForegroundColor = GetRandomColor();
            DisplayLogo(Console.ForegroundColor);
            lastColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;

            DisplayMenuText();

            Console.ForegroundColor = GetRandomColor();

            while (Console.ForegroundColor == lastColor)
            {
                Console.ForegroundColor = GetRandomColor();
            }

            lastColor = Console.ForegroundColor;

            Program.DisableInput();

            Program.acceptInput = true;

            //Change logo colors
            while (true)
            {
                DisplayLogo(lastColor);

                Console.ForegroundColor = GetRandomColor();

                while (Console.ForegroundColor == lastColor)
                {
                    Console.ForegroundColor = GetRandomColor();
                }

                lastColor = Console.ForegroundColor;
            }
        }

        public static void DisplayMainMenu()
        {
            ConsoleColor lastColor;

            if (!audioMuted)
            {
                Console.Title = "Space Invaders | Main Menu (Sound: ON)";
            }
            else
            {
                Console.Title = "Space Invaders | Main Menu (Sound: OFF)";
            }

            Console.SetCursorPosition(menuX + 46, menuY - 5);
            Console.Write("By Ben Morelli-Kirshner");

            Console.SetCursorPosition(menuX + 20, menuY - 2);
            Console.Write("Select your choice by pressing the corresponding number on the keyboard.");

            Console.SetCursorPosition(menuX + 49, menuY + 1);
            Console.Write("1. Start Game");

            Console.SetCursorPosition(menuX + 49, menuY + 3);
            Console.Write("2. Instructions");

            Console.SetCursorPosition(menuX + 49, menuY + 5);
            Console.Write("3. Credits");

            Console.SetCursorPosition(menuX + 49, menuY + 7);
            Console.Write("4. Exit");

            Console.SetCursorPosition(2, 48);
            Console.Write("Created by Ubisoft Montreal ;)");

            Console.SetCursorPosition(109, 48);
            Console.Write("Sound is playing ('N' to change song, 'M' to mute)");

            Console.ForegroundColor = GetRandomColor();
            DisplayLogoInstantly(Console.ForegroundColor);
            lastColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;

            Console.ForegroundColor = GetRandomColor();

            while (Console.ForegroundColor == lastColor)
            {
                Console.ForegroundColor = GetRandomColor();
            }

            lastColor = Console.ForegroundColor;

            Program.DisableInput();

            //Change logo colors
            while (true)
            {
                DisplayLogo(lastColor);

                Console.ForegroundColor = GetRandomColor();

                while (Console.ForegroundColor == lastColor)
                {
                    Console.ForegroundColor = GetRandomColor();
                }

                lastColor = Console.ForegroundColor;
            }
        }

        static void DisplayLogo(ConsoleColor dlogocolor)
        {
            for (int x = 0; x < logoLines.Length; x++)
            {
                //Move cursor one line down
                Console.SetCursorPosition(menuX, logoY + x);

                //Clear line
                for (int i = 0; i < logoLines[x].Length; i++)
                {
                    //If user presses a key, detect it
                    if (Console.KeyAvailable && Program.acceptInput == true)
                    {
                        MenuDetectKey(dlogocolor);
                    }
                    Console.Write(Game.backgroundTexture);
                    Console.SetCursorPosition(menuX + i + 1, logoY + x);
                }

                Console.SetCursorPosition(menuX, logoY + x);
                Thread.Sleep(logoColorScrollSpeed);

                //Write line
                Console.Write(logoLines[x]);
            }
        }

        static void DisplayLogoInstantly(ConsoleColor dlogocolor)
        {
            for (int x = 0; x < logoLines.Length; x++)
            {
                //Move cursor one line down
                Console.SetCursorPosition(menuX, logoY + x);

                //Clear line
                for (int i = 0; i < logoLines[x].Length; i++)
                {
                    //If user presses a key, detect it
                    if (Console.KeyAvailable && Program.acceptInput == true)
                    {
                        MenuDetectKey(dlogocolor);
                    }
                    Console.Write(Game.backgroundTexture);
                    Console.SetCursorPosition(menuX + i + 1, logoY + x);
                }

                Console.SetCursorPosition(menuX, logoY + x);

                //Write line
                Console.Write(logoLines[x]);
            }
        }

        static void DisplayMenuText()
        {
            Console.SetCursorPosition(menuX + 46, menuY - 5);
            Program.TypeText("By Ben Morelli-Kirshner");

            Console.SetCursorPosition(menuX + 20, menuY - 2);
            Program.TypeText("Select your choice by pressing the corresponding number on the keyboard.");

            Console.SetCursorPosition(menuX + 49, menuY + 1);
            Program.TypeText("1. Start Game");

            Console.SetCursorPosition(menuX + 49, menuY + 3);
            Program.TypeText("2. Instructions");

            Console.SetCursorPosition(menuX + 49, menuY + 5);
            Program.TypeText("3. Credits");

            Console.SetCursorPosition(menuX + 49, menuY + 7);
            Program.TypeText("4. Exit");

            Console.SetCursorPosition(2, 48);
            Program.TypeText("Created by Ubisoft Montreal ;)");

            Console.SetCursorPosition(109, 48);
            Program.TypeText("Sound is playing ('N' to change song, 'M' to mute)");
        }

        static void MenuDetectKey(ConsoleColor dlogocolor)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                    Game.StartGame();
                    break;
                case ConsoleKey.D2:
                    DisplayInstructions();
                    break;
                case ConsoleKey.D3:
                    //credits
                    break;
                case ConsoleKey.D4:
                    Environment.Exit(0);
                    break;
                case ConsoleKey.M:
                    if (!Program.audioMuted)
                    {
                        Console.Title = "Space Invaders | Main Menu (Sound: OFF)";
                        Console.SetCursorPosition(109, 48);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Sound is muted ('M' to play)                     ");
                        Console.ForegroundColor = dlogocolor;
                        Program.audioMuted = true;
                        Program.musicPlaying = false;
                    }
                    else
                    {
                        Console.Title = "Space Invaders | Main Menu (Sound: ON)";
                        Console.SetCursorPosition(109, 48);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Sound is playing ('N' to change song, 'M' to mute)");
                        Console.ForegroundColor = dlogocolor;
                        Program.audioMuted = false;
                        Program.musicPlaying = true;
                    }
                    break;
                case ConsoleKey.N:
                    if (!Program.changeSong)
                    {
                        Program.changeSong = true;
                    }
                    else
                    {
                        Program.changeSong = false;
                    }
                    break;
                default:
                    break;
            }
        }

        static ConsoleColor GetRandomColor()
        {
            ConsoleColor color;

            color = (ConsoleColor)(Program.random.Next(Enum.GetNames(typeof(ConsoleColor)).Length));

            while (color == ConsoleColor.Black)
            {
                color = (ConsoleColor)(Program.random.Next(Enum.GetNames(typeof(ConsoleColor)).Length));
            }

            return color;
        }

        //static void SetWindowSize()
        //{
        //    while (true)
        //    {
        //        if (WINDOW_HEIGHT != WINDOW_HEIGHT || WINDOW_WIDTH != WINDOW_WIDTH)
        //        {
        //            Console.CursorVisible = false;
        //            Console.SetCursorPosition(0, 0);
        //            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
        //        }
        //    }
        //}
    }
}