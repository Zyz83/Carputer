using System;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;


namespace CarputerWPF
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer clock;
        
        public MainWindow()
        {
            InitializeComponent();
            clock = new System.Timers.Timer(1000);
            clock.Elapsed += timeUpdate;
            clock.Start();
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += RunSystemCheck;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            bw.RunWorkerAsync();
            tabs.SelectedItem = tab_Main;
            Unosquare.FFME.MediaElement.FFmpegDirectory = @"c:\ffmpeg";
        }

        private void RunSystemCheck(object sender, EventArgs e)
        {
            UpdateSystemLabel("Check");
            ResetSystemLabels();
            Type t = typeof(PowerStatus);
            PropertyInfo[] pi = t.GetProperties();
            Thread.Sleep(1000);
            var ni = (from n in NetworkInterface.GetAllNetworkInterfaces() where n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && n.Name == "Wi-Fi" select n).FirstOrDefault();
            switch (ni.OperationalStatus)
            {
                case OperationalStatus.Up:
                    UpdateWifiLabel("OK");
                    break;
                case OperationalStatus.Down:
                    UpdateWifiLabel("N/C");
                    break;
                default:
                    UpdateWifiLabel("N/A");
                    break;
            }
            Thread.Sleep(500);
            UpdateSystemTempLabel(Temperature.GetTemp());
            Thread.Sleep(500);
            UpdateBatteryLabel((decimal.Parse(pi.Where(p => p.Name == "BatteryLifePercent").FirstOrDefault().GetValue(SystemInformation.PowerStatus, null).ToString()) * 100).ToString("N0"));
            Thread.Sleep(500);
            UpdateGPSLabel("N/A");
            Thread.Sleep(500);
            var bl = (from n in NetworkInterface.GetAllNetworkInterfaces() where n.NetworkInterfaceType == NetworkInterfaceType.Ethernet && n.Name.StartsWith("Bluetooth") select n).FirstOrDefault();
            if (bl != null)
            {
                switch (bl.OperationalStatus)
                {
                    case OperationalStatus.Up:
                        UpdateBluetoothLabel("OK");
                        break;
                    case OperationalStatus.Down:
                        UpdateBluetoothLabel("N/C");
                        break;
                    default:
                        UpdateBluetoothLabel("N/A");
                        break;
                }
            }
            else
                UpdateBluetoothLabel("N/A");
            Thread.Sleep(1000);
        }


        #region Button
        private void ActionButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var imageObject = ((Image)sender);
            if(imageObject.Name == "lightAndHorn")
            {
                ResetAction(light);
            }
            else if (imageObject.Name == "light")
            {
                ResetAction(lightAndHorn);
            }
            var img = ((Image)sender).Source.ToString().Replace("{", "").Replace("}", "");
            img = img.EndsWith(".png") ? img.Replace(".png", ".gif") : img.Replace(".gif", ".png");
            SetButtonImage((Image)sender, img);
            //CallAction(((Image)sender).Name);
        }
        
        private void ViewButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var img = ((Image)sender).Source.ToString().Replace("{", "").Replace("}", "");
            img = img.EndsWith("W.png") ? img.Replace("W.png", ".png") : img.Replace(".png", "W.png");
            foreach (Image i in mainGrid.Children.OfType<Image>())
                ResetView(i);
            SetButtonImage(sender, img);
            OpenView(((Image)sender).Name);
        }

        private void CallAction(string name)
        {
            // TODO: call actions in WCF
            throw new NotImplementedException();
        }

        private void OpenView(string name)
        {
            switch (name)
            {
                case "car":
                    if (tabs.SelectedItem != tab_Car)
                        tabs.SelectedItem = tab_Car;
                    else
                        tabs.SelectedItem = tab_Main;
                    break;
                case "cam":
                    if (tabs.SelectedItem != tab_Cam)
                        tabs.SelectedItem = tab_Cam;
                    else
                        tabs.SelectedItem = tab_Main;
                    break;
                case "nav":
                    if (tabs.SelectedItem != tab_Nav)
                        tabs.SelectedItem = tab_Nav;
                    else
                        tabs.SelectedItem = tab_Main;
                    break;
                case "question":
                    if (tabs.SelectedItem != tab_Question)
                        tabs.SelectedItem = tab_Question;
                    else
                        tabs.SelectedItem = tab_Main;
                    break;
                case "message":
                    if (tabs.SelectedItem != tab_Message)
                        tabs.SelectedItem = tab_Message;
                    else
                        tabs.SelectedItem = tab_Main;
                    break;
                case "media":
                    if (tabs.SelectedItem != tab_Media)
                        tabs.SelectedItem = tab_Media;
                    else
                        tabs.SelectedItem = tab_Main;
                    break;
                case "con":
                    if (tabs.SelectedItem != tab_Con)
                        tabs.SelectedItem = tab_Con;
                    else
                        tabs.SelectedItem = tab_Main;
                    break;
                default:
                    tabs.SelectedItem = tab_Main;
                    break;
            }
        }

        private void ResetAction(Image image)
        {
            var png = image.Source.ToString().Replace("{", "").Replace("}", "").Replace(".gif", ".png");
            SetButtonImage(image, png);
        }
        private void ResetView(Image image)
        {
            var png = image.Source.ToString().Replace("{", "").Replace("}", "").Replace("W.png", ".png");
            SetButtonImage(image, png);
        }

        private static void SetButtonImage(object sender, string img)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(img);
            image.EndInit();
            ImageBehavior.SetAnimatedSource((Image)sender, image);
        }
        #endregion

        #region Label Updates
        private void UpdateSystemLabel(string text)
        {
            la_System_Value.Dispatcher.Invoke(delegate () { la_System_Value.Content = text; });
        }

        private void ResetSystemLabels()
        {
                la_Battery_Value.Dispatcher.Invoke(delegate () { la_Battery_Value.Content = string.Empty; });
                la_Systemtemp_Value.Dispatcher.Invoke(delegate () { la_Systemtemp_Value.Content = string.Empty; });
                la_Bluetooth_Value.Dispatcher.Invoke(delegate () { la_Bluetooth_Value.Content = string.Empty; });
                la_GPS_Value.Dispatcher.Invoke(delegate () { la_GPS_Value.Content = string.Empty; });
                la_Wifi_Value.Dispatcher.Invoke(delegate () { la_Wifi_Value.Content = string.Empty; });
        }

        private void UpdateBatteryLabel(string text)
        {
            la_Battery_Value.Dispatcher.Invoke(delegate () { la_Battery_Value.Content = text; });
        }
        private void UpdateSystemTempLabel(string text)
        {
            la_Systemtemp_Value.Dispatcher.Invoke(delegate () { la_Systemtemp_Value.Content = text; });
        }
        private void UpdateBluetoothLabel(string text)
        {
            la_Bluetooth_Value.Dispatcher.Invoke(delegate () { la_Bluetooth_Value.Content = text; });
        }
        private void UpdateGPSLabel(string text)
        {
            la_GPS_Value.Dispatcher.Invoke(delegate () { la_GPS_Value.Content = text; });
        }
        private void UpdateWifiLabel(string text)
        {
            la_Wifi_Value.Dispatcher.Invoke(delegate () { la_Wifi_Value.Content = text; });
        }

        public delegate void UpdateLabelCallback(object label, string value);
        private void UpdateLabel(object label, string value)
        {
            ((System.Windows.Controls.Label)label).Content = value;
        }
        #endregion

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateSystemLabel("OK");
        }
        private void timeUpdate(object sender, EventArgs e)
        {
            var tod = DateTime.Now.TimeOfDay;
            la_HHMM.Dispatcher.Invoke(new UpdateLabelCallback(this.UpdateLabel),
                new object[] { la_HHMM, $"{tod.Hours.ToString("D2")}:{tod.Minutes.ToString("D2")}" });
            la_SS.Dispatcher.Invoke(new UpdateLabelCallback(this.UpdateLabel),
                new object[] { la_SS, $"{tod.Seconds.ToString("D2")}" });
        }

        private void Exit_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
