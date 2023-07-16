using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FishBot
{
    public static class Bot
    {
        public static IntPtr GameWindowHandle;

        public static bool IsBotStarted = false;
        public static bool IsShiftCarStarted = false;

        public static int SleepTime = -1; // in secs
        private static bool CaptchaAppeared = false;

        private static int RobotCounter = 0;

        public static string CaptchaPosInQueue = "0";

        public static string Status = "BotStopped";

        public static int Counter = 0;

        public static int Income = 0;
        public static bool UseMaxPrice = false;

        public static bool IsGameFound = false;

        public static Thread MainThread;
        public static Thread SleepThread;

        public static bool StoppedByUser = false;

        private static bool ResHelpSent = false;
        private static bool RedBoatHelpSent = false;

        private static void TapRobot()
        {
            KeysManager.SendLeftClick(GameWindowHandle,
                new Point(Config.CaptchaTextRect.X + Config.CaptchaTextRect.Width / 2,
                Config.CaptchaTextRect.Y + Config.CaptchaTextRect.Height / 2));

            SleepTime = (++RobotCounter) * 600;

            Status = "BotGoneSleep";
        }

        public static void StartShiftCar()
        {
            MainThread = new Thread(() =>
            {
                Status = "BotStarted";

                IsShiftCarStarted = true;

                var wBounds = Capture.GetWindowBounds(GameWindowHandle);
                Config.GameResolution = wBounds.Width + "x" + wBounds.Height;

                if (IsGameFound && !Config.Resolutions.ContainsKey(Config.GameResolution))
                {
                    for (int i = -5; i < 6; i++)
                        for (int j = -5; j < 6; j++)
                            if (Config.Resolutions.ContainsKey((wBounds.Width + i) + "x" + (wBounds.Height + j)))
                            {
                                Config.GameResolution = (wBounds.Width + i) + "x" + (wBounds.Height + j);

                                break;
                            }

                    if (!Config.Resolutions.ContainsKey(Config.GameResolution))
                    {
                        Status = "ResolutionNotFound";

                        if (Config.VKNotificate && !ResHelpSent)
                        {
                            SendVKMessage($"Ваше разрешение ({Config.GameResolution}) не поддерживается!\nСмотрите список поддерживаемых разрешений в инструкции, также проверьте, включен ли оконный режим в игре\nТакже, разрешение в Windows должно быть либо больше, либо равно тому, что установлено в игре.\n\nЕсли же у вас все работало, то это ошибка RAGE MP, а не бота. Поможет перезагрузка ПК.");

                            ResHelpSent = true;
                        }

                        Stop();
                        return;
                    }
                }

                Bitmap Bitmap = new Bitmap(1, 1);

                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.H, 2500);

                Inventory BoatInventory = new Inventory(Config.BoatInvRect);
                Inventory BoatBag = new Inventory(new Rectangle(Config.BoatInvRect.X + 595, Config.BoatInvRect.Y, Config.BoatInvRect.Width, Config.BoatInvRect.Height));

                Status = "ShiftingFish";

                if (BoatInventory.Shift(BoatBag, Config.FishList.Select(x => x.Name).ToList(), GameWindowHandle))
                {
                    KeysManager.SendLeftClick(GameWindowHandle, Config.BoatClosePoint);

                    Status = "ShiftFinish";
                }
                else
                    Status = "NoCarOrBoatNear";

                Stop();
            });

            MainThread.Start();
        }

        private static void OpenInventory()
        {
            KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I, 500);

            DateTime dt = DateTime.Now;

            Bitmap Bitmap = new Bitmap(1, 1);

            do
            {
                Bitmap = Bitmap.Update(GameWindowHandle, Config.SmilesRect);

                if (DateTime.Now.Subtract(dt).TotalMilliseconds > 10000)
                {
                    dt = DateTime.Now;

                    KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I, 500);
                }

                Thread.Sleep(100);
            }
            while (Bitmap.FindPixel(Config.CanFishColor).Bool);
        }

        

        public static void Start()
        {
            if (SleepTime != -1)
            {
                Status = "BotStillSleeping";

                Stop();
                return;
            }

            IsBotStarted = true;

            int OperationCounter = 0;

            MainThread?.Abort();

            MainThread = new Thread(() =>
            {
                var wBounds = Capture.GetWindowBounds(GameWindowHandle);
                Config.GameResolution = wBounds.Width + "x" + wBounds.Height;

                if (IsGameFound && !Config.Resolutions.ContainsKey(Config.GameResolution))
                {
                    for (int i = -5; i < 6; i++)
                        for (int j = -5; j < 6; j++)
                            if (Config.Resolutions.ContainsKey((wBounds.Width + i) + "x" + (wBounds.Height + j)))
                            {
                                Config.GameResolution = (wBounds.Width + i) + "x" + (wBounds.Height + j);

                                break;
                            }

                    if (!Config.Resolutions.ContainsKey(Config.GameResolution))
                    {
                        Status = "ResolutionNotFound";

                        if (Config.VKNotificate && !ResHelpSent)
                        {
                            SendVKMessage($"Ваше разрешение ({Config.GameResolution}) не поддерживается!\nСмотрите список поддерживаемых разрешений в инструкции, также проверьте, включен ли оконный режим в игре\nТакже, разрешение в Windows должно быть либо больше, либо равно тому, что установлено в игре.\n\nЕсли же у вас все работало, то это ошибка RAGE MP, а не бота. Поможет перезагрузка ПК.");

                            ResHelpSent = true;
                        }

                        Stop();
                        return;
                    }
                }

                Bitmap Bitmap = new Bitmap(1, 1);

                bool FoundBag = false;

                bool Hungry = false;
                bool BadMood = false;

                Inventory Inventory = null;
                (double Weight, double MaxWeight) weight = (0, 10);

                // Due to admins detect bot by static click latency we make it random
                Random random = new Random();

                while (MainThread.IsAlive && IsGameFound)
                {
                    try
                    {
                        Status = "BotStarted";

                        Bitmap = Bitmap.Update(GameWindowHandle, Config.SmilesRect);

                        if (Config.HungerCheck && Bitmap.FindPixel(Config.HungerColor).Bool)
                            if (Config.AutoEat)
                                Hungry = true;
                            else
                            {
                                Status = "Hungry";

                                if (Config.VKNotificate && OperationCounter > 0)
                                    SendVKMessage(Config.Phrases["Hungry"].Text + "А возможно, у вас оранжевая лодка или слева на экране боту мешает оранжевый цвет, тогда поверните камеру!");

                                break;
                            }

                        if (Config.MoodCheck && Bitmap.FindPixel(Config.MoodColor).Bool)
                            if (Config.AutoSmoke)
                                BadMood = true;
                            else
                            {
                                Status = "BadMood";

                                if (Config.VKNotificate && OperationCounter > 0)
                                    SendVKMessage(Config.Phrases["BadMood"].Text + " А возможно, у вас фиолетовая лодка или слева на экране боту мешает фиолетовый цвет, тогда поверните камеру!");

                                break;
                            }

                        if (!Bitmap.FindPixel(Config.CanFishColor).Bool)
                        {
                            Status = "NoFishing";

                            if (Config.VKNotificate && OperationCounter > 0)
                                SendVKMessage(Config.Phrases["NoFishingVK"].Text + "А возможно, у вас желтая лодка или слева на экране боту мешает желтый цвет, тогда поверните камеру!");

                            break;
                        }

                        if (Config.BlockMouse)
                            KeysManager.EnableInput(GameWindowHandle, false);

                        OpenInventory();

                        if (Config.RealisticOn)
                            Thread.Sleep(random.Next(1, 3) * 1000 + random.Next(250, 1000));

                        if (OperationCounter == 0 || weight.Weight >= 9.5)
                        {
                            if (Bitmap.Update(GameWindowHandle, Config.InvWBagWeightRect).GetWeight(Config.BinarizationWeight).Weight == -1)
                                FoundBag = false;
                            else
                                FoundBag = true;

                            Inventory = new Inventory(FoundBag ? Config.InventoryWBagRect : Config.InventoryRect);

                            Inventory.UpdateItems(Bitmap.Update(GameWindowHandle, Inventory.Rectangle));
                        }

                        if (Config.AutoEat && Hungry)
                        {
                            foreach (var food in Config.FoodList)
                                if (Inventory.Items[food.ToLower()].Count > 0)
                                {
                                    Status = "Eating";

                                    KeysManager.SendRightClick(GameWindowHandle, Inventory.Items[food.ToLower()][0]);

                                    Thread.Sleep(8000);

                                    OpenInventory();

                                    Hungry = false;

                                    break;
                                }

                            if (Hungry)
                            {
                                Status = "NoFood";

                                if (Config.VKNotificate && OperationCounter > 0)
                                    SendVKMessage(Config.Phrases["NoFood"].Text);

                                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I);
                                break;
                            }
                        }

                        if (Config.AutoSmoke && BadMood)
                        {
                            bool foundSigs = Inventory.Items["сигареты"].Count > 0;
                            bool foundVape = Inventory.Items["вейп"].Count > 0 || Inventory.Items["веип"].Count > 0;

                            if (foundSigs || foundVape)
                            {
                                Status = "Smoking";

                                string name = foundSigs ? "сигареты" : "вейп";

                                KeysManager.SendRightClick(GameWindowHandle, Inventory.Items[name][0]);

                                Thread.Sleep(foundSigs ? 35000 : 5000);

                                /*                            Thread.Sleep(1000);

                                                            if (Bitmap.Update(GameWindowHandle, Config.SmilesRect).FindPixel(Config.MoodColor).Bool)
                                                            {
                                                                Thread.Sleep(35000);

                                                                OpenInventory();

                                                                Inventory.UpdateItems(Bitmap.Update(GameWindowHandle, Inventory.Rectangle));

                                                                if (Inventory.Items[name].Count > 0)
                                                                    KeysManager.SendRightClick(GameWindowHandle, Inventory.Items[name][0]);
                                                                else
                                                                {
                                                                    Status = "NoSigs";

                                                                    if (Config.VKNotificate && OperationCounter > 0)
                                                                        SendVKMessage(Config.Phrases[name].Text);

                                                                    KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I);
                                                                    break;
                                                                }
                                                            }*/

                                OpenInventory();

                                BadMood = false;
                            }
                            else
                            {
                                Status = "NoSigs";

                                if (Config.VKNotificate && OperationCounter > 0)
                                    SendVKMessage(Config.Phrases["NoSigs"].Text);

                                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I);
                                break;
                            }
                        }

                        if (OperationCounter == 0)
                        {
                            // Первая проверка на удочку
                            if (Inventory.Items["удочка"].Count == 0)
                            {
                                Status = "RodNotFound1";

                                Thread.Sleep(4000);

                                Inventory.UpdateItems(Bitmap.Update(GameWindowHandle, Inventory.Rectangle));
                            }

                            // Последняя проверка на удочку
                            if (Inventory.Items["удочка"].Count == 0)
                            {
                                Status = "RodNotFound2";

                                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I);
                                break;
                            }

                            if (Inventory.Items["приманка"].Count == 0)
                            {
                                Status = "BaitNotFound";

                                if (Config.SoundsOn)
                                    SystemSounds.Beep.Play();

                                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I);
                                break;
                            }
                        }

                        var newWeight = Bitmap.Update(GameWindowHandle, FoundBag ? Config.InvWBagWeightRect : Config.InvWeightRect).GetWeight(Config.BinarizationWeight);

                        if (weight.Weight < newWeight.Weight)
                            weight.Weight = newWeight.Weight;

                        if (Config.OverweightBehaviour && weight.Weight >= weight.MaxWeight - 0.5)
                        {
                            if (Config.DropFish)
                            {
                                Status = "DroppingFish";

                                foreach (var fish in Config.FishToDropList)
                                    foreach (var x in Inventory.Items[fish])
                                    {
                                        KeysManager.DragMouse(GameWindowHandle, x, Point.Empty);
                                        KeysManager.SendLeftClick(GameWindowHandle, Config.DropMaxPoint);
                                        KeysManager.SendLeftClick(GameWindowHandle, Config.DropApplyPoint);

                                        Thread.Sleep(500);
                                    }

                                Thread.Sleep(500);

                                weight = Bitmap.Update(GameWindowHandle, FoundBag ? Config.InvWBagWeightRect : Config.InvWeightRect).GetWeight(Config.BinarizationWeight);
                            }


                            if (Config.ShiftBoat && (weight.Weight >= weight.MaxWeight - 0.5))
                            {
                                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I, 200);
                                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.H, 2000);

                                Inventory BoatInventory = new Inventory(Config.BoatInvRect);
                                Inventory BoatBag = new Inventory(new Rectangle(Config.BoatInvRect.X + 595, Config.BoatInvRect.Y, Config.BoatInvRect.Width, Config.BoatInvRect.Height));

                                BoatInventory.UpdateItems(Bitmap.Update(GameWindowHandle, Config.BoatInvRect));

                                var Fishes = Config.FishList.Select(x => x.Name);

                                if (BoatInventory.Items.Where(x => (x.Value.Count > 0 && Fishes.Contains(x.Key))).Count() > 0)
                                {
                                    Status = "ShiftingFish";

                                    BoatInventory.Shift(BoatBag, Config.FishList.Select(x => x.Name).ToList(), GameWindowHandle);
                                }

                                KeysManager.SendLeftClick(GameWindowHandle, Config.BoatClosePoint);
                                Thread.Sleep(200);
                                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I, 2000);

                                weight = Bitmap.Update(GameWindowHandle, FoundBag ? Config.InvWBagWeightRect : Config.InvWeightRect).GetWeight(Config.BinarizationWeight);
                            }

                            if (FoundBag && (weight.Weight >= weight.MaxWeight - 0.5))
                            {
                                Status = "ShiftingFish";

                                Inventory Bag = new Inventory(Config.BagRect);

                                Inventory.Shift(Bag, Config.FishList.Select(x => x.Name).ToList(), GameWindowHandle);

                                var oldWeight = weight;

                                weight = Bitmap.Update(GameWindowHandle, Config.InvWBagWeightRect).GetWeight(Config.BinarizationWeight);

                                if (oldWeight.Weight <= weight.Weight)
                                {
                                    Status = "NoPlaceInBag";

                                    if (Config.SoundsOn)
                                        SystemSounds.Beep.Play();

                                    if (Config.VKNotificate && OperationCounter > 0)
                                        SendVKMessage(Config.Phrases["NoPlaceInBag"].Text);

                                    KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I);
                                    break;
                                }
                            }
                            else if (weight.Weight >= weight.MaxWeight - 0.5)
                            {
                                Status = "Overweight";

                                if (Config.SoundsOn)
                                    SystemSounds.Beep.Play();

                                if (Config.VKNotificate && OperationCounter > 0)
                                    SendVKMessage(Config.Phrases["Overweight"].Text);

                                KeysManager.SendKey(GameWindowHandle, KeysManager.VK.I);
                                break;
                            }

                            OperationCounter = 0;
                        }

                        KeysManager.SendRightClick(GameWindowHandle, Inventory.Items["удочка"][0]);

                        Status = "Fishing";

                        Thread.Sleep(5000);

                        if (Bitmap.Update(GameWindowHandle, Config.CaptchaTextRect).BinarizeImage(Config.BinarizationCaptcha).GetText(Capture.TesseractAPI).Contains("робот") || Bitmap.Update(GameWindowHandle, Config.CaptchaTextRect).BinarizeImage(Config.BinarizationCaptcha + 10).GetText(Capture.TesseractAPI).Contains("робот"))
                        {
                            if (Config.TapRobot)
                            {
                                TapRobot();

                                break;
                            }

                            CaptchaAppeared = true;

                            if (Config.BypassCaptcha && Config.CaptchaTimeRemain > 0 && MainWindow.CaptchaSolver != null)
                            {
                                Status = "CaptchaProcessing";

                                var image = Bitmap.Update(GameWindowHandle, Config.CaptchaRect);

                                KeysManager.SendLeftClick(GameWindowHandle, Config.CaptchaStartPoint);

                                Thread.Sleep(500);

                                string captchaId = null;

                                try
                                {
                                    int counter = 0;

                                    image.Save(System.IO.Path.GetTempPath() + "frfxcn.png");

                                    var captcha = new Captcha.Normal(System.IO.Path.GetTempPath() + "frfxcn.png");

                                    captcha.SetCalc(false);
                                    captcha.SetNumeric(1);
                                    captcha.SetCaseSensitive(false);
                                    captcha.SetMinLen(4);
                                    captcha.SetMaxLen(10);
                                    captcha.SetSoftId(3426);

                                    MainWindow.CaptchaSolver.Solve(captcha);

                                    captchaId = captcha.Id;

                                    for (int i = 0; i < captcha.Code.Length; i++)
                                    {
                                        if (char.IsDigit(captcha.Code[i]))
                                        {
                                            KeysManager.SendKey(GameWindowHandle, (int)KeysManager.VK.Number0 + Convert.ToInt32(captcha.Code[i]), 500);

                                            counter++;
                                        }
                                        else
                                            throw new Exception();
                                    }

                                    KeysManager.SendLeftClick(GameWindowHandle, Config.CaptchaApplyPoint);

                                    Thread.Sleep(2500);

                                    if (Bitmap.Update(GameWindowHandle, Config.FishRect).BinarizeImage(Config.BinarizationFish).GetText(Capture.TesseractAPI).ToLower().Contains("блок"))
                                    {
                                        Status = "CaptchaError";

                                        if (Config.VKNotificate)
                                            SendVKMessage(Config.Phrases["CaptchaError"].Text + " Бот ушел в сон! Возможно, это ошибка, тогда бот продолжит рыбачить сам.");

                                        if (captchaId != null)
                                            try
                                            {
                                                System.IO.File.Delete(System.IO.Path.GetTempPath() + "frfxcn.png");

                                                MainWindow.CaptchaSolver.Report(captchaId, false);
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                        TapRobot();

                                        break;
                                    }
                                    else
                                    {
                                        CaptchaAppeared = false;

                                        Status = "CaptchaCompleted";

                                        if (captchaId != null)
                                            try
                                            {
                                                System.IO.File.Delete(System.IO.Path.GetTempPath() + "frfxcn.png");

                                                MainWindow.CaptchaSolver.Report(captchaId, true);
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Status = "CaptchaError";

                                    if (Config.VKNotificate)
                                        SendVKMessage(Config.Phrases["CaptchaError"].Text + " Пройдите её самостоятельно!");

                                    if (captchaId != null)
                                        try
                                        {
                                            System.IO.File.Delete(System.IO.Path.GetTempPath() + "frfxcn.png");

                                            MainWindow.CaptchaSolver.Report(captchaId, false);
                                        }
                                        catch (Exception eyx)
                                        {

                                        }
                                }

                                Thread.Sleep(1000);
                            }
                            else
                            {
                                if (Config.SoundsOn)
                                    SystemSounds.Beep.Play();
                                if (Config.VKNotificate)
                                    SendVKMessage(Config.Phrases["CaptchaAppeared"].Text);

                                Status = "CaptchaAppeared";
                            }
                        }

                        if (Bitmap.Update(GameWindowHandle, Config.ResultRect).FindPixel(Config.FaultColor, 1).Bool)
                        {
                            if (Config.SoundsOn)
                                SystemSounds.Beep.Play();

                            if (Bitmap.Update(GameWindowHandle, Config.FishRect).BinarizeImage(Config.BinarizationFish).GetText(Capture.TesseractAPI).ToLower().Contains("приманк"))
                            {
                                Status = "BaitNotFound";

                                if (Config.SoundsOn)
                                    SystemSounds.Beep.Play();

                                if (Config.VKNotificate)
                                    SendVKMessage(Config.Phrases["BaitNotFound"].Text);
                            }
                            else
                            {
                                Status = "ActionRestricted";

                                if (Config.VKNotificate && OperationCounter > 0 && !RedBoatHelpSent)
                                {
                                    SendVKMessage("Ошибка 'Действие заблокировано' может возникать, если у вас появилось уведомление ошибки в игре или у вас красная лодка\nВ первом случае, просто запустите бота еще раз, во втором - отверните камеру персонажа от красного цвета в нижней части экрана, например, в сторону воды или неба");

                                    RedBoatHelpSent = true;
                                }
                            }

                            break;
                        }

                        if (!CaptchaAppeared)
                            Status = "Fishing";

                        DateTime dt = DateTime.Now;

                        do
                        {
                            Bitmap = Bitmap.Update(GameWindowHandle, Config.MouseRect);

                            if (!CaptchaAppeared && DateTime.Now.Subtract(dt).TotalMilliseconds > 60000)
                                break;

                            Thread.Sleep(75);
                        }
                        while (!Bitmap.FindPixel(Config.ClickColor, 2, 0.2f, 0.2f).Bool);

                        if (!CaptchaAppeared && DateTime.Now.Subtract(dt).TotalMilliseconds > 60000)
                        {
                            if (Config.SoundsOn)
                                SystemSounds.Beep.Play();

                            if (Config.VKNotificate)
                                SendVKMessage(Config.Phrases["SomethingWentWrong"].Text);

                            Status = "BotStopped";

                            break;
                        }

                        CaptchaAppeared = false;

                        Status = "FishingOut";

                        int sleepOffset = Config.ClickLatency < 50 ? 0 : 10;

                        if (Config.RealisticOn)
                            sleepOffset = 50;

                        do
                        {
                            Bitmap = Bitmap.Update(GameWindowHandle, Config.MouseRect);

                            if (sleepOffset == 0)
                            {
                                Point zeroPoint = new Point(0, 0);

                                while (Bitmap.FindPixel(Config.ClickColor, 2).Bool)
                                {
                                    KeysManager.SendLeftClick(GameWindowHandle, zeroPoint);

                                    Thread.Sleep(Config.ClickLatency);

                                    Bitmap = Bitmap.Update(GameWindowHandle, Config.MouseRect);
                                }
                            }
                            else
                            {

                                while (Bitmap.FindPixel(Config.ClickColor, 2).Bool)
                                {
                                    KeysManager.SendLeftClick(GameWindowHandle, new Point(random.Next(0, 10), random.Next(0, 10)));

                                    Thread.Sleep(random.Next(Config.ClickLatency, Config.ClickLatency + sleepOffset));

                                    Bitmap = Bitmap.Update(GameWindowHandle, Config.MouseRect);
                                }
                            }
                        }
                        while (Bitmap.FindPixel(Config.ClickColor, 2).Bool || Bitmap.FindPixel(Config.WaitColor, 2).Bool);

                        Thread.Sleep(500);

                        Bitmap = Bitmap.Update(GameWindowHandle, Config.ResultRect);

                        var successPos = Bitmap.FindPixel(Config.SuccessColor, 4);
                        var faultPos = Bitmap.FindPixel(Config.FaultColor, 4);

                        if (successPos.Bool)
                        {
                            Status = "FishingSuccess";

                            Counter++;

                            string fish = Bitmap.Update(GameWindowHandle, Config.FishRect).BinarizeImage(Config.BinarizationFish).GetText(Capture.TesseractAPI).ToLower();

                            if (fish != null)
                            {
                                if (fish.Contains("осетр"))
                                    fish = "осётр";

                                Fish item = Config.FishList.Find(x => fish.Contains(x.Name));

                                if (item != null)
                                {
                                    item.Amount++;

                                    weight.Weight += item.Weight - 0.01;

                                    if (Config.SubtractBait)
                                        Income += UseMaxPrice ? item.MaxCost - 35 : item.MinCost - 35;
                                    else
                                        Income += UseMaxPrice ? item.MaxCost : item.MinCost;

                                    foreach (var item1 in Config.FishList)
                                        item1.Chance = ((float)item1.Amount / Counter).ToString("0.00");
                                }
                            }
                        }
                        else if (faultPos.Bool)
                        {
                            Status = "FishingFault";

                            Income -= 35;

                            weight.Weight -= 0.01;

                            break;
                        }

                        OperationCounter++;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                Stop();
            });

            MainThread.Start();
        }

        public static void Stop()
        {
            if (IsBotStarted || IsShiftCarStarted)
            {
                if (IsGameFound)
                    KeysManager.EnableInput(GameWindowHandle);

                if (!IsShiftCarStarted)
                    IsBotStarted = false;
                else
                    IsShiftCarStarted = false;

                if (MainThread.IsAlive)
                    MainThread.Abort();
            }
        }

        public static void SendVKMessage(string message)
        {
            try
            {
                string base64Key = Convert.ToBase64String(Encoding.UTF8.GetBytes(Config.Key + "  " + DateTime.Now.ToString("YYYY/mm/dd HH:mm:ss")));

                var request = (HttpWebRequest)WebRequest.Create(Properties.Resources.url + "/vkbot.php");
                var postData =
                    $"notification=true&data={HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(base64Key + AES.EncryptString(Config.HWID + message, base64Key)))}";

                var data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                var responseString = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd();
            }
            catch
            {

            }
        }
    }
}