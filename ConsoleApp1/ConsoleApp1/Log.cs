using System;
using System.IO;

namespace VT_Editor
{
    class Log
    {
        static FileStream outStream;
        static StreamWriter writer;
        static TextWriter stdOut = Console.Out;

        public static void CreateOrOpenLogFile()
        {
            try
            {
                if (!File.Exists("./VT Editor LogFile.txt"))
                {
                    outStream = new FileStream("./VT Editor LogFile.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    writer = new StreamWriter(outStream);
                }
                else
                {
                    outStream = new FileStream("./VT Editor LogFile.txt", FileMode.Append, FileAccess.Write);
                    writer = new StreamWriter(outStream);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Vt Editor LogFile.txt for writing");
                Console.WriteLine(e.Message);
                Console.ReadLine();
                return;
            }
        }

        public static void Write(string logFileInput)
        {
            Console.SetOut(writer);
            Console.Write(logFileInput);
            Console.SetOut(stdOut);
        }

        public static void WriteLine(string logFileInput)
        {
            Console.SetOut(writer);
            Console.WriteLine(logFileInput);
            Console.SetOut(stdOut);
        }

        public static void Close()
        {
            writer.Close();
            outStream.Close();
        }
    }
}
