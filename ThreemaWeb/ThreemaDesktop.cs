using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Structs;
using CefSharp.WinForms;


namespace ThreemaWeb
{
    public partial class ThreemaDesktop : Form
    {
        private ChromiumWebBrowser chrome;
        private bool canReceiveNotifications;
        private long last_Notification = UnixTimeNow();
        public ThreemaDesktop()
        {
            InitializeComponent();
            IniChrome();
        }

        private void IniChrome()
        {


            CefSettings settings = new CefSettings
            {
                CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF",
                RemoteDebuggingPort = 8088
            };

            Cef.Initialize(settings);

            chrome = new ChromiumWebBrowser("https://web.threema.ch/");
            chrome.Dock = DockStyle.Fill;
            chrome.ConsoleMessage += this.Web_ConsoleMessage;
            chrome.FrameLoadEnd += this.Loaded;
            chrome_panel.Controls.Add(chrome);
            
            chrome.TitleChanged += this.OnTitleChanged;

        }

        private void Loaded(object sender, FrameLoadEndEventArgs e)
        {
            canReceiveNotifications = true;
        }

        private void OnTitleChanged(object sender, CefSharp.TitleChangedEventArgs e)
        {
            if (!canReceiveNotifications || this.Text.Equals(e.Title) || (ThreemaDesktop.ActiveForm == this))
                return;

            this.Invoke(new Action(() =>
            {
                this.Text = e.Title;
                notifyIcon.BalloonTipTitle = e.Title;
                notifyIcon.BalloonTipText = "New Message";
                notifyIcon.ShowBalloonTip(2500);
            }));

        }

        private void ThreemaWeb_FormClosing(object sender, FormClosingEventArgs e) => Cef.Shutdown();

        void Web_ConsoleMessage(object sender, CefSharp.ConsoleMessageEventArgs e)
        {
            if ((ThreemaDesktop.ActiveForm == this))
                return;

            if (e.Message.Contains("New Message"))
            {
                // Wenn Letzte Nachricht (100) neue Nachricht (101) -> nicht abspielen
                if (this.last_Notification + 10 < UnixTimeNow() )
                {
                    System.Console.WriteLine("New Message");
                    this.last_Notification = UnixTimeNow();
                    return;
                }
                return;
            }
        }

        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }
        /*
        public void OnAddressChanged(IWebBrowser browserControl, AddressChangedEventArgs addressChangedArgs)
        {
            Console.WriteLine("Adress Changed");
        }

        public bool OnAutoResize(IWebBrowser browserControl, IBrowser browser, CefSharp.Structs.Size newSize)
        {
            Console.WriteLine("Resize");
            return false;
        }

        public void OnTitleChanged(IWebBrowser browserControl, TitleChangedEventArgs titleChangedArgs)
        {
            Console.WriteLine("---------New Message");
        }

        public void OnFaviconUrlChange(IWebBrowser browserControl, IBrowser browser, IList<string> urls)
        {
            Console.WriteLine("Favi Changed");
        }

        public void OnFullscreenModeChange(IWebBrowser browserControl, IBrowser browser, bool fullscreen)
        {
            Console.WriteLine("FullScreenNow");
        }

        public bool OnTooltipChanged(IWebBrowser browserControl, ref string text)
        {
            Console.WriteLine("ToolTipChange");
            return false;
        }

        public void OnStatusMessage(IWebBrowser browserControl, StatusMessageEventArgs statusMessageArgs)
        {
            Console.WriteLine("statusmsg");
        }

        public bool OnConsoleMessage(IWebBrowser browserControl, ConsoleMessageEventArgs consoleMessageArgs)
        {
            return false; 
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chrome.Back();
        }
        */



    }
}
