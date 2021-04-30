using System;

namespace GZipArchiver
{
    public static class ConsoleProxy
    {
        private static readonly object Locker = new object();
        private static int _currentPosition = 1;
        static ConsoleProxy()
        {
            lock (Locker)
            {
                Console.CursorVisible = false;
                Console.CursorLeft = 0;
                Console.Write("[");
                Console.CursorLeft = 31;
                Console.Write("] 0%");
                Console.CursorLeft = 1;
            }
        }

        public static void DrawProgressBar(int percent)
        {
            lock (Locker)
            {
                Console.CursorLeft = _currentPosition;
                for (int i = _currentPosition - 1; i < 30 * percent / 100; i++)
                {
                    Console.Write("=");
                    _currentPosition++;
                }
                Console.CursorLeft = 33;
                Console.Write(string.Format("{0}%", percent));
            }
        }

        public static void ShowMessage(string message)
        {
            lock (Locker)
            {
                Console.WriteLine();
                Console.Write(message);
            }
        }

    }
}
