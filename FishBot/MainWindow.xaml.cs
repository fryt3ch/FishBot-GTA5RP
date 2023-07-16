using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FishBot
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static System.Windows.Forms.NotifyIcon NIcon;

        private static SolidColorBrush GreenBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
        private static SolidColorBrush RedBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));


        private Thread CheckGameThread;
        private Thread KeysListenerThread;
        private Thread BotCheckThread;
        private Thread TimeRemainThread;
        private Thread CounterUpdateThread;

        public MainWindow() => InitializeComponent();

        public static Captcha.TwoCaptcha CaptchaSolver;

        protected void WindowLoaded(object sender, RoutedEventArgs e)
        {
            bool ShowedUpdateFound = false;

            if (Config.CaptchaApiKey.Length > 0)
                CaptchaSolver = new Captcha.TwoCaptcha(Config.CaptchaApiKey);

            TimeRemainThread = new Thread(async () =>
            {
                while (TimeRemainThread.IsAlive)
                {
                    if (Config.TimeRemain > 0)
                        TimeRemainLabel?.Dispatcher.Invoke((Action)(() =>
                        {
                            if (Config.TimeRemain >= 15_000_000)
                            {
                                if (TimeRemainLabel.Visibility != Visibility.Hidden)
                                    TimeRemainLabel.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                if (TimeRemainLabel.Visibility != Visibility.Visible)
                                    TimeRemainLabel.Visibility = Visibility.Visible;

                                TimeRemainLabel.Content = "Подписка: " + Config.TimeRemain / 3600 + " час.";
                            }
                        }));

                    if (Config.CaptchaTimeRemain <= 0)
                        bypass_captcha?.Dispatcher.Invoke((Action)(() =>
                        {
                            if ((string)bypass_captcha.ToolTip != "У вас нет подписки!")
                            {
                                Config.UpdateParameter("bypass_captcha", "False");

                                bypass_captcha.IsChecked = false;
                                bypass_captcha.ToolTip = "У вас нет подписки!";
                            }
                        }));
                    else if (Config.CaptchaTimeRemain > 0)
                    {
                        if (Config.CaptchaApiKey.Length == 0)
                        {
                            bypass_captcha?.Dispatcher.Invoke((Action)(() =>
                            {
                                bypass_captcha.ToolTip = $"Ваша подписка активна, но API ключ не задан!\nПроверьте config.xml";
                            }));
                        }
                        else
                        {
                            try
                            {
                                var balance = CaptchaSolver.Balance();

                                bypass_captcha?.Dispatcher.Invoke((Action)(() =>
                                {
                                    bypass_captcha.ToolTip = $"Ваша подписка активна!\nAPI ключ: {Config.CaptchaApiKey}\nБаланс: {balance} руб.";
                                }));
                            }
                            catch (Exception ex)
                            {
                                bypass_captcha?.Dispatcher.Invoke((Action)(() =>
                                {
                                    bypass_captcha.ToolTip = $"Ваша подписка активна, но возможно, ваш API ключ неверный!&#xD;&#xA;API ключ: {Config.CaptchaApiKey}";
                                }));
                            }
                        }
                    }

                    Thread.Sleep(60000);

                    if (!ShowedUpdateFound)
                    {
                        if (LoginWindow.NewVersionFound())
                        {
                            ShowedUpdateFound = true;

                            MessageBox.Show("Найдено обновление!", "FishBot | Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
    
                    try
                    {
                        string encodedKey = Config.Key + TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now).AddHours(3).ToString("yyyy/MM/dd HH:mm:ss");

                        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                        {
                            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(encodedKey);
                            byte[] hashBytes = md5.ComputeHash(inputBytes);

                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < hashBytes.Length; i++)
                                sb.Append(hashBytes[i].ToString("X2"));

                            encodedKey = sb.ToString() + "1";
                        }

                        var request = (HttpWebRequest)WebRequest.Create(Properties.Resources.url + "/auth.php");
                        var postData =
                            "menu_auth=true&xml_required=false&" +
                            $"data={HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(encodedKey)) + AES.EncryptString(Config.Key + Config.HWID, encodedKey)))}";

                        var data = Encoding.ASCII.GetBytes(postData);

                        request.Method = "POST";
                        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = data.Length;

                        using (var stream = request.GetRequestStream())
                            stream.Write(data, 0, data.Length);

                        var responseString = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd();

                        int resultBr = responseString.IndexOf("<RESULT>") + "<RESULT>".Length;
                        responseString = responseString.Substring(resultBr, responseString.IndexOf("</RESULT>") - resultBr);
                        responseString = AES.DecryptString(responseString.Substring(43), responseString.Substring(0, 43));

                        if (responseString.Contains("<TIMESTAMP>"))
                        {
                            int timestampBr = responseString.IndexOf("<TIMESTAMP") + "<TIMESTAMP>".Length;
                            string timestamp = responseString.Substring(timestampBr, responseString.IndexOf("</TIMESTAMP>") - timestampBr);

                            var currentDate = TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.Now).AddHours(3);
                            var actualDate = DateTime.Parse(timestamp);

                            if (currentDate.Subtract(actualDate).TotalSeconds > 3600)
                            {
                                MessageBox.Show("Токен более недействителен!", "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                                Bot.SendVKMessage("У вас появилась ошибка - Токен более недействителен\nПопробуйте установить время на ПК автоматически, а часовой пояс поставить вручную\nТакже, попробуйте синхронизировать время, чтобы оно соответствовало реальному!");

                                NameLabel.Dispatcher.Invoke((Action)(() => CloseApp()));
                            }

                            int secsBr = responseString.IndexOf("<BOT_SECS>") + "<BOT_SECS>".Length;
                            int newTimeRemain = int.Parse(responseString.Substring(secsBr, responseString.IndexOf("</BOT_SECS>") - secsBr));

                            Config.TimeRemain = newTimeRemain;

                            secsBr = responseString.IndexOf("<CAPTCHA_SECS>") + "<CAPTCHA_SECS>".Length;
                            newTimeRemain = int.Parse(responseString.Substring(secsBr, responseString.IndexOf("</CAPTCHA_SECS>") - secsBr));

                            Config.CaptchaTimeRemain = newTimeRemain;
                        }
                        else
                        {
                            if (Bot.IsBotStarted)
                                Bot.Stop();

                            if (KeysListenerThread.IsAlive)
                                KeysListenerThread.Abort();

                            MessageBox.Show(responseString, "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            Bot.SendVKMessage(responseString);

                            NameLabel.Dispatcher.Invoke((Action)(() => CloseApp()));
                        }
                    }
                    catch
                    {
                        if (Bot.IsBotStarted)
                            Bot.Stop();

                        if (KeysListenerThread.IsAlive)
                            KeysListenerThread.Abort();

                        MessageBox.Show("Ошибка соединения!", "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        NameLabel.Dispatcher.Invoke((Action)(() => CloseApp()));
                    }
                }
            });

            TimeRemainThread.Start();

            //Icon in tray
            NIcon = new System.Windows.Forms.NotifyIcon();
            NIcon.Icon = Properties.Resources.Icon;
            NIcon.Visible = true;

            NIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            NIcon.ContextMenuStrip.Items.Add("Скрыть панель (F8)", null, HideItemClicked);
            NIcon.ContextMenuStrip.Items.Add("Выйти (F9)", null, CloseItemClicked);

            //Hide app from Alt+TAB
            var helper = new WindowInteropHelper(this).Handle;
            SetWindowLong(helper, -20, (GetWindowLong(helper, -20) | 0x00000080) & ~0x00040000);

            NameLabel.ToolTip = $"Версия: {Properties.Resources.version}\n\nvk.com/fishbot_gta5rp";

            drop_fish.ToolTip = (string)drop_fish.ToolTip + "\nСписок: " + string.Join(" ", Config.FishToDropList.Select(x => char.ToUpper(x[0]) + x.Substring(1)));

            SleepLabel.Visibility = Visibility.Hidden;
            SettingsGrid.Visibility = Visibility.Collapsed;

            FishMenuGrid.Visibility = Visibility.Collapsed;
            IncomeLabel.Visibility = Visibility.Visible;

            FishMenuData.ItemsSource = Config.FishList.Select(x => x.Name);

            foreach (var col in FishMenuData.Columns)
                col.IsReadOnly = true;

            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height * 1.5;

            UpdateControls();

            CheckGameThread = new Thread(() =>
            {
                while (CheckGameThread.IsAlive)
                {
                    Process[] ps = Process.GetProcessesByName("GTA5");

                    if (ps.Length != 0)
                    {
                        Bot.GameWindowHandle = ps[0].MainWindowHandle;
                        Bot.IsGameFound = true;
                    }
                    else
                    {
                        if (Bot.IsBotStarted || Bot.IsShiftCarStarted)
                        {
                            //Bot.Status = "GameNotFound";
                            //Bot.Stop();
                        }

                        Bot.IsGameFound = false;
                    }

                    Bot.IsGameFound = true; //

                    Thread.Sleep(1000);
                }
            });

            CheckGameThread.Start();

            KeysListenerThread = new Thread(() =>
            {
                while (KeysListenerThread.IsAlive)
                {
                    if (KeysManager.IsKeyPressed(System.Windows.Forms.Keys.F4))
                    {
                        if (!Bot.IsBotStarted && !Bot.IsShiftCarStarted)
                        {
                            Bot.StoppedByUser = false;

                            if (Bot.IsGameFound)
                                Bot.Start();
                            else
                                Bot.Status = "GameNotFound";
                        }
                        else if (Bot.IsBotStarted)
                        {
                            Bot.StoppedByUser = true;

                            Bot.Status = "BotStopped";
                            Bot.Stop();
                        }
                    }
                    else if (KeysManager.IsKeyPressed(System.Windows.Forms.Keys.F5))
                    {
                        if (!Bot.IsBotStarted && !Bot.IsShiftCarStarted)
                        {
                            if (Bot.IsGameFound)
                                Bot.StartShiftCar();
                            else
                                Bot.Status = "GameNotFound";
                        }
                        else if (Bot.IsShiftCarStarted)
                        {
                            Bot.Status = "BotStopped";
                            Bot.Stop();
                        }
                    }
                    else if (KeysManager.IsKeyPressed(System.Windows.Forms.Keys.Insert))
                        StatusGrid.Dispatcher.Invoke((Action)(() => StatusGridMouseRightClick(this, null)));
                    else if (KeysManager.IsKeyPressed(System.Windows.Forms.Keys.F8))
                        StatusGrid.Dispatcher.Invoke((Action)(() => HideItemClicked(NIcon.ContextMenuStrip.Items[0], null)));
                    else if (KeysManager.IsKeyPressed(System.Windows.Forms.Keys.F9))
                        StatusGrid.Dispatcher.Invoke((Action)(() => CloseApp()));

                    Thread.Sleep(500);
                }
            });

            KeysListenerThread.Start();

            Bot.SleepThread = new Thread(() =>
            {
                while (Bot.SleepThread.IsAlive)
                {
                    SleepLabel.Dispatcher.Invoke((Action)(() =>
                    {
                        if (Bot.SleepTime > 0)
                        {
                            SleepLabel.Visibility = Visibility.Visible;
                            SleepLabel.Content = "Сон: " + Bot.SleepTime--.ToString() + " сек.";
                        }
                        else if (Bot.SleepTime == 0)
                        {
                            Bot.SleepTime = -1;

                            SleepLabel.Visibility = Visibility.Hidden;

                            Bot.Start();
                        }
                    }));

                    Thread.Sleep(1000);
                }
            });

            Bot.SleepThread.Start();

            BotCheckThread = new Thread(() =>
            {
                while (BotCheckThread.IsAlive)
                {
                    StatusText.Dispatcher.Invoke((Action)(() =>
                    {
                        if (StatusText.Text != Config.Phrases[Bot.Status].Text)
                        {
                            System.Drawing.Color col = Config.Phrases[Bot.Status].Color;

                            StatusText.Text = (Bot.Status != "CaptchaInQueue") ? Config.Phrases[Bot.Status].Text : Config.Phrases[Bot.Status].Text + Bot.CaptchaPosInQueue;
                            StatusText.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(col.R, col.G, col.B));
                        }
                    }));

                    Thread.Sleep(500);
                }
            });

            BotCheckThread.Start();

            CounterUpdateThread = new Thread(() =>
            {
                while (CounterUpdateThread.IsAlive)
                {
                    if (Bot.IsBotStarted)
                        IncomeLabel.Dispatcher.Invoke((Action)(() =>
                        {
                            IncomeLabel.Foreground = Bot.Income >= 0 ? GreenBrush : RedBrush;
                            IncomeLabel.Content = Bot.Income + " $";
                        }));

                    if (FishMenuGrid.Visibility == Visibility.Visible)
                    {
                        FishMenuData.Dispatcher.Invoke((Action)(() =>
                        {
                            FishMenuData.ItemsSource = null;
                            FishMenuData.ItemsSource = Config.FishList;

                            foreach (var col in FishMenuData.Columns)
                                col.IsReadOnly = true;
                        }));
                    }

                    Thread.Sleep(1000);
                }
            });

            CounterUpdateThread.Start();
        }

        private void UpdateControls()
        {
            MenuGrid.Opacity = Config.Opacity;

            opacity.Value = Config.Opacity;
            OpacityLabel.Content = Config.Opacity * 100 + " %";

            foreach (CheckBox cb in CheckboxGrid.Children)
                cb.IsChecked = bool.Parse(Config.GetNodeValue(cb.Name));

            click_latency.Value = double.Parse(Config.GetNodeValue("click_latency"), System.Globalization.NumberStyles.Any);
            SliderLabel.Content = click_latency.Value + " ms.";
        }

        private void HideItemClicked(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolStripItem item = (System.Windows.Forms.ToolStripItem)sender;

            bool isVisible = Visibility == Visibility.Visible;

            item.Text = isVisible ? "Показать панель (F8)" : "Скрыть панель (F8)";

            Visibility = Visibility.Visible;

            DoubleAnimation anim = new DoubleAnimation();

            anim.From = isVisible ? Config.Opacity : 0;
            anim.To = isVisible ? 0 : Config.Opacity;
            anim.Duration = TimeSpan.FromMilliseconds(250);
            anim.Completed += (object sender1, EventArgs e1) => Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;

            this.BeginAnimation(Window.OpacityProperty, anim, HandoffBehavior.SnapshotAndReplace);
        }

        private void CloseApp()
        {
            Bot.Stop();

            CheckGameThread.Abort();
            KeysListenerThread.Abort();
            Bot.SleepThread.Abort();
            BotCheckThread.Abort();
            CounterUpdateThread.Abort();
            TimeRemainThread.Abort();

            NIcon.Dispose();

            Config.fs.Close();

            Application.Current.Shutdown();
        }

        private void CloseItemClicked(object sender, EventArgs e) => CloseApp();

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }

        protected void StatusGridMouseRightClick(object sender, MouseButtonEventArgs e)
        {
            if (FishMenuGrid.Visibility == Visibility.Visible)
                FishMenuGrid.Visibility = Visibility.Collapsed;

            bool isVisible = SettingsGrid.Visibility == Visibility.Visible;

            SettingsGrid.Visibility = Visibility.Visible;

            DoubleAnimation anim = new DoubleAnimation();

            anim.From = isVisible ? Config.Opacity : 0;
            anim.To = isVisible ? 0 : Config.Opacity;
            anim.Duration = TimeSpan.FromMilliseconds(250);
            anim.Completed += (object sender1, EventArgs e1) => SettingsGrid.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;

            SettingsGrid.BeginAnimation(Grid.OpacityProperty, anim, HandoffBehavior.SnapshotAndReplace);
        }

        protected void ConfigLabelDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                using (Process fileopener = new Process())
                {
                    fileopener.StartInfo.FileName = "explorer";
                    fileopener.StartInfo.Arguments = "\"" + Directory.GetCurrentDirectory() + @"\config.xml" + "\"";
                    fileopener.Start();
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                Config.LoadDefaultConfig();

                UpdateControls();
            }
        }

        protected void CheckBoxClicked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).Name == "bypass_captcha" && Config.CaptchaTimeRemain <= 0)
            {
                bypass_captcha.IsChecked = false;

                return;
            }

            Config.UpdateParameter(((CheckBox)sender).Name, ((CheckBox)sender).IsChecked.ToString());

            if (((CheckBox)sender).Name == "tap_robot" && (bool)bypass_captcha.IsChecked && (bool)tap_robot.IsChecked)
            {
                Config.UpdateParameter("bypass_captcha", false.ToString());
                bypass_captcha.IsChecked = false;
            }
            else if (((CheckBox)sender).Name == "bypass_captcha" && (bool)tap_robot.IsChecked && (bool)bypass_captcha.IsChecked)
            {
                Config.UpdateParameter("tap_robot", false.ToString());
                tap_robot.IsChecked = false;
            }
            else if (((CheckBox)sender).Name == "hunger_check" && (bool)auto_eat.IsChecked && !(bool)hunger_check.IsChecked)
            {
                Config.UpdateParameter("auto_eat", false.ToString());
                auto_eat.IsChecked = false;
            }
            else if (((CheckBox)sender).Name == "mood_check" && (bool)auto_smoke.IsChecked && !(bool)mood_check.IsChecked)
            {
                Config.UpdateParameter("auto_smoke", false.ToString());
                auto_smoke.IsChecked = false;
            }
            else if (((CheckBox)sender).Name == "auto_eat" && (bool)((CheckBox)sender).IsChecked)
            {
                Config.UpdateParameter("hunger_check", true.ToString());
                hunger_check.IsChecked = true;
            }
            else if (((CheckBox)sender).Name == "auto_smoke" && (bool)((CheckBox)sender).IsChecked)
            {
                Config.UpdateParameter("mood_check", true.ToString());
                mood_check.IsChecked = true;
            }
            else if (((CheckBox)sender).Name == "subtract_bait")
                RecountIncome();
        }

        protected void SliderChanged(object sender, RoutedEventArgs e)
        {
            if (SliderLabel != null)
            {
                SliderLabel.Content = ((Slider)sender).Value + " ms";

                Config.UpdateParameter(((Slider)sender).Name, ((int)Math.Floor(((Slider)sender).Value)).ToString());
            }
        }

        protected void OpacitySliderChanged(object sender, RoutedEventArgs e)
        {
            if (OpacityLabel != null)
            {
                MenuGrid.Opacity = ((Slider)sender).Value;

                OpacityLabel.Content = ((Slider)sender).Value * 100 + " %";

                Config.UpdateParameter(((Slider)sender).Name, ((int)Math.Floor(((Slider)sender).Value * 100)).ToString());
            }
        }
        protected void IncomeLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (SettingsGrid.Visibility == Visibility.Visible)
                    SettingsGrid.Visibility = Visibility.Collapsed;

                bool isVisible = FishMenuGrid.Visibility == Visibility.Visible;

                FishMenuGrid.Visibility = Visibility.Visible;

                DoubleAnimation anim = new DoubleAnimation();

                anim.From = isVisible ? Config.Opacity : 0;
                anim.To = isVisible ? 0 : Config.Opacity;
                anim.Duration = TimeSpan.FromMilliseconds(250);
                anim.Completed += (object sender1, EventArgs e1) => FishMenuGrid.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;

                FishMenuGrid.BeginAnimation(Grid.OpacityProperty, anim, HandoffBehavior.SnapshotAndReplace);
            }
        }

        protected void IncomeLabelDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Bot.Income = 0;
                Bot.Counter = 0;

                foreach (var fish in Config.FishList)
                {
                    fish.Amount = 0;
                    fish.Chance = "0,00";
                }

                IncomeLabel.Content = "0 $";
                IncomeLabel.Foreground = GreenBrush;
            }
            else if (e.ChangedButton == MouseButton.Right)
                RecountIncome();
        }

        protected void RecountIncome()
        {
            Bot.Income = 0;

            Bot.UseMaxPrice = !Bot.UseMaxPrice;

            if (Config.FishList.Any((x) => x.Amount > 0))
            {
                if (Bot.UseMaxPrice)
                    foreach (var fish in Config.FishList)
                        Bot.Income += fish.Amount * (fish.MaxCost - (Config.SubtractBait ? 35 : 0));
                else
                    foreach (var fish in Config.FishList)
                        Bot.Income += fish.Amount * (fish.MinCost - (Config.SubtractBait ? 35 : 0));
            }

            IncomeLabel.Content = Bot.Income + " $";
            IncomeLabel.Foreground = GreenBrush;
        }
    }
}
