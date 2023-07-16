using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tesseract;

namespace FishBot
{
    public partial class LoginWindow : Window
    {
        private List<string> LastKeys;

        public LoginWindow()
        {
            LastKeys = new List<string>();

            try
            {
                Capture.TesseractAPI = new TesseractEngine(AppDomain.CurrentDomain.BaseDirectory + "x86", "rus", EngineMode.LstmOnly);
                Capture.TesseractAPI.SetVariable("user_defined_dpi", "300");
                //Capture.TesseractAPI.SetVariable("tessedit_char_whitelist", Encoding.GetEncoding(1251).GetString(Encoding.UTF8.GetBytes("СсТтЕеРрЛлЯяДдЬьОоЁёТтУуНнЦцЧчЫыЙйАаМмКкФфГгЫыПпЙйВвИиБб:")));

                Capture.TesseractNumAPI = new TesseractEngine(AppDomain.CurrentDomain.BaseDirectory + "x86", "digitsall_layer", EngineMode.LstmOnly);
                Capture.TesseractNumAPI.SetVariable("user_defined_dpi", "300");
                Capture.TesseractNumAPI.SetVariable("tessedit_char_whitelist", Encoding.GetEncoding(1251).GetString(Encoding.UTF8.GetBytes("1234567890./")));
            }
            catch (Exception ex)
            {
                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "error-log.txt", Encoding.UTF8.GetBytes(ex.Message));

                MessageBox.Show(
                        "Не найдены необходимые библиотеки!\nВы точно не удаляли DLL файлы или папку x86?\n\n1) Если включён MSI Afterburner, то выключите его, бот с ним не работает\n2) Если включен антивирус, выключите его и добавьте бота в исключения\n3) Скачайте бота заново, удалив текущие файлы",
                        "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current.Shutdown();
            }

            try
            {
                if (!File.Exists(Config.ConfigPath))
                    File.WriteAllBytes(Config.ConfigPath, Encoding.UTF8.GetBytes(Properties.Resources.config));

                Config.LoadConfig();
            }
            catch
            {

                File.Delete(Config.ConfigPath);
                File.WriteAllBytes(Config.ConfigPath, Encoding.UTF8.GetBytes(Properties.Resources.config));

                Config.LoadConfig();

                MessageBox.Show(
                            "Файл конфигурации бота был восстановлен в стандартный вид (либо файл был недоступен)",
                            "FishBot | Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                Config.fs = File.Open(Config.ConfigPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                Config.sw = new StreamWriter(Config.fs);

                InitializeComponent();
            }
        }

        protected void LoginWindowLoaded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (File.Exists(Directory.GetCurrentDirectory() + @"\updater.exe"))
                File.Delete(Directory.GetCurrentDirectory() + @"\updater.exe");

            if (NewVersionFound())
            {
                MessageBox.Show("Найдено обновление!", "FishBot | Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
                
                try
                {
                    File.WriteAllBytes(Directory.GetCurrentDirectory() + @"\updater.exe", Properties.Resources.updater);

                    ProcessStartInfo startInfo = new ProcessStartInfo(Directory.GetCurrentDirectory() + @"\updater.exe", Properties.Resources.url);
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;

                    Process.Start(startInfo);

                    Application.Current.Shutdown();
                }
                catch
                {
                    MessageBox.Show("Ошибка запуска обновленя!", "FishBot | Обновление", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            string key = Config.UserKeyNode.Attributes.GetNamedItem("value").Value;

            KeyInputField.Text = key;

            if (ValidateKey(key))
            {
                LoginLabelMouseLeftDown(this, null);
            }

            try
            {
                if (LastKeys == null)
                    return;

                Visibility = Visibility.Visible;

                DoubleAnimation anim = new DoubleAnimation();

                anim.From = 0;
                anim.To = 1;
                anim.Duration = TimeSpan.FromMilliseconds(500);
                anim.Completed += (object sender1, EventArgs e1) => Visibility = Visibility.Visible;

                this.BeginAnimation(Window.OpacityProperty, anim, HandoffBehavior.SnapshotAndReplace);
            }
            catch (Exception ex)
            {

            }
        }

        public static bool NewVersionFound()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Properties.Resources.url + "/update.php");
                var postData =
                    "check_update=true";

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                var responseString = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd();

                int resultBr = responseString.IndexOf("<VERSION>") + "<VERSION>".Length;
                string newVersion = responseString.Substring(resultBr, responseString.IndexOf("</VERSION>") - resultBr);

                if (Properties.Resources.version != newVersion)
                    return true; // new version found
                else
                    return false; // latest version
            }
            catch
            {
                MessageBox.Show("Ошибка соединения!", "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }

        protected bool ValidateKey(string key) => (new Regex(@"^FISHBOT_[A-Z0-9]{16}$")).IsMatch(key);

        protected void MoveWindow(object sender, MouseEventArgs e) => this.DragMove();

        protected void LoginLabelMouseLeftDown(object sender, MouseEventArgs e)
        {
            string curKey = KeyInputField.Text;

            if (LastKeys.Contains(curKey))
            {
                MessageBox.Show("Вы уже вводили этот ключ и он неверный!", "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (!ValidateKey(curKey))
            {
                LastKeys.Add(curKey);

                MessageBox.Show("Неверный ключ!", "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Config.HWID = HWID.Value();
            Config.Key = curKey;

            Debug.WriteLine(Config.HWID);

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
                    "menu_auth=true&xml_required=true&" +
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

                        return;
                    }


                    int secsBr = responseString.IndexOf("<BOT_SECS>") + "<BOT_SECS>".Length;
                    Config.TimeRemain = int.Parse(responseString.Substring(secsBr, responseString.IndexOf("</BOT_SECS>") - secsBr));

                    secsBr = responseString.IndexOf("<CAPTCHA_SECS>") + "<CAPTCHA_SECS>".Length;
                    Config.CaptchaTimeRemain = int.Parse(responseString.Substring(secsBr, responseString.IndexOf("</CAPTCHA_SECS>") - secsBr));

                    Config.UserKeyNode.Attributes.GetNamedItem("value").Value = curKey;
                    Config.SaveConfig();

                    try
                    {
                        int xmlBr = responseString.IndexOf("<XML>") + "<XML>".Length;
                        string xml = responseString.Substring(xmlBr, responseString.IndexOf("</XML>") - xmlBr);

                        Config.LoadResolutions(xml);

                        MainWindow mw = new MainWindow();
                        mw.ShowActivated = true;

                        mw.Show();
                        this.Close();

                        LastKeys.Clear();
                        LastKeys = null;
                    }
                    catch
                    {
                        MessageBox.Show("Конфигурация разрешений экрана не загружена!", "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(responseString, "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    if (responseString.Equals("Неверный ключ!"))
                        LastKeys.Add(curKey);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка соединения!", "FishBot | Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }         
        }

        protected void ExitLabelMouseLeftDown(object sender, MouseEventArgs e) => Application.Current.Shutdown();

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        protected void LabelMouseEnter(object sender, MouseEventArgs e)
        {
            var col = ((SolidColorBrush)((Label)sender).Foreground).Color;

            ((Label)sender).Foreground = new SolidColorBrush(Color.FromArgb((byte)(col.A - 100), col.R, col.G, col.B));
        }

        protected void LabelMouseLeave(object sender, MouseEventArgs e)
        {
            var col = ((SolidColorBrush)((Label)sender).Foreground).Color;

            ((Label)sender).Foreground = new SolidColorBrush(Color.FromArgb((byte)(col.A + 100), col.R, col.G, col.B));
        }
    }
}
