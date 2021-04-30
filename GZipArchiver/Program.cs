using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GZipArchiver
{
    class Program
    {
        private static GZip _archiver;
        private static Handler _handler;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string textException = "";
            var timer = new Stopwatch();
            _handler += ConsoleCtrlCheck;
            SetConsoleCtrlHandler(_handler);
            if (args.Length == 3 && InputValidation(args, ref textException))
            {
                if (args[0].ToLower() == "compress")
                    _archiver = new GZipCompressor(args[1], args[2]);
                else
                    _archiver = new GZipDecompressor(args[1], args[2]);
                _archiver.OnEventHandler(ShowCurrentProgress);
                timer.Start();
                _archiver.Start();
                timer.Stop();
                if (_archiver.ResultProcess == ProcessingResult.OK)
                {
                    ConsoleProxy.ShowMessage(String.Format(
                        "Обработка файла успешно завершена. Время работы = {0} секунды.",
                        (int) timer.Elapsed.TotalSeconds));
                }
            }
            else
            {
                ShowCurrentProgress(textException == ""
                    ? "Неверный формат входных данных!"
                    : textException);
            }
            Console.CursorVisible = true;
        }

        private static bool ConsoleCtrlCheck()
        {
            _archiver.Stop();
            return true;
        }
        private static void ShowCurrentProgress(string info)
        {
            int percent = 0;
            if (int.TryParse(info, out percent))
            {
                ConsoleProxy.DrawProgressBar(percent);
            }
            else
            {
                _archiver?.Stop();
                ConsoleProxy.ShowMessage(info);
            }
        }

        private static bool InputValidation(string[] data, ref string textException)
        {
            if (data[0].ToLower() == "compress")
            {
                if (!File.Exists(data[1])
                    && !File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + data[1]))
                {
                    textException = "Ошибка! Входной файл не найден. ";
                    return false;
                }
                if (Path.GetExtension(data[2]) != ".gz"
                    || (Path.GetExtension(Path.GetFileNameWithoutExtension(data[2])) != "" &&
                    Path.GetExtension(Path.GetFileNameWithoutExtension(data[2])) != Path.GetExtension(data[1])))
                {
                    textException = "Ошибка! Выходной файл имеет неверное расширение. ";
                    return false;
                }

                if (!Directory.Exists(Path.GetDirectoryName(data[2]))
                    && Path.GetDirectoryName(data[2]) != "")
                {
                    textException = "Ошибка! Путь для выходного файла указан не верно. ";
                    return false;
                }
                return true;
            }
            if (data[0].ToLower() == "decompress")
            {
                if (!File.Exists(data[1])
                    && !File.Exists(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + data[1]))
                {
                    textException = "Ошибка! Входной файл не найден. ";
                    return false;
                }
                if (Path.GetExtension(data[1]) != ".gz")
                {
                    textException = "Ошибка! Входной файл имеет неверное расширение. ";
                    return false;
                }
                if (Path.GetExtension(Path.GetFileNameWithoutExtension(data[1])) != "" 
                     && Path.GetExtension(Path.GetFileNameWithoutExtension(data[1])) != Path.GetExtension(data[2]))
                {
                    textException = "Ошибка! Выходной файл имеет неверное расширение. ";
                    return false;
                }
                if (!Directory.Exists(Path.GetDirectoryName(data[2]))
                    && Path.GetDirectoryName(data[2]) != "")
                {
                    textException = "Ошибка! Путь для выходного файла указан не верно. ";
                    return false;
                }
                return true;
            }
            else
            {
                textException = "Ошибка! Неверная команда. ";
                return false;
            }
        }

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(Handler handler);
        public delegate bool Handler();
    }

}
