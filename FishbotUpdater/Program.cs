using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace FishbotUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(2000);

            try
            {
                try
                {
                    foreach (Process proc in Process.GetProcessesByName("firefox"))
                    {
                        proc.Kill();
                        proc.WaitForExit();
                        proc.Dispose();
                    }

                    WebClient wc = new WebClient();
                    wc.DownloadFile(args[0] + "/firefox.exe", AppDomain.CurrentDomain.BaseDirectory + @"\firefox.exe");

                    Console.WriteLine("Новая версия успешно скачана!");

                    Thread.Sleep(2000);

                    Process.Start(new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + @"\firefox.exe"));
                }
                catch
                {
                    Console.WriteLine("Не удалось скачать обновление!\nВозможно вы не закрыли программу.");

                    Thread.Sleep(2000);
                }
            }
            catch
            {
                Console.WriteLine("Ошибка соединения с сервером!");

                Thread.Sleep(2000);
            }
        }
    }
}
