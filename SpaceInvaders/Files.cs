using System;
using System.IO;
using System.Reflection;

namespace SpaceInvaders
{
    //Everything related to file management (saving audio files)
    class Files
    {
        //Path to save game audio + settings (AppData\Roaming\Space Invaders)
        public static string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Space Invaders\\";

        static int filesDone = 0;
        static string[] resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

        public static void SaveAudioToDisk()
        {
            //Save all audio from resources and settings file to the disk

            int X = Program.WINDOW_WIDTH / 2 - 19;
            int Y = Program.WINDOW_HEIGHT / 2;

            int filesCount = Directory.GetFiles(savePath + "\\audio\\", "*", SearchOption.TopDirectoryOnly).Length;
            filesDone = filesCount;

            for (int i = 0; i < resources.Length - 1; i++)
            {
                Console.Clear();
                Console.SetCursorPosition(X, Y);
                Console.Write("Writing audio files to disk... (" + filesDone + "/" + (resources.Length - 1) + " done)");
                Console.Title = "Writing audio files... (" + filesDone + "/" + (resources.Length - 1) + ")";

                if (!File.Exists(savePath + "\\audio\\" + resources[i + 1]))
                {
                    SaveResourceToDisk(resources[i + 1]);
                }

                //SAVE XML FILE HERE?
            }

        }

        static void SaveResourceToDisk(string dresource)
        {
            //Write single application resource (audio file) to disk

            //Create stream of bytes from the specified resource
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(dresource);

            //Create new file to write resource to
            FileStream fileStream = new FileStream(savePath + "\\audio\\" + dresource, FileMode.CreateNew);

            for (int i = 0; i < stream.Length; i++)
            {
                //Write all bytes from the stream to the file
                fileStream.WriteByte((byte)stream.ReadByte());
            }

            fileStream.Close();
            filesDone++;
        }
    }
}