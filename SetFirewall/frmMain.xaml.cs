using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetFwTypeLib;

namespace SetFirewall
{
    /// <summary>
    /// frmMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class frmMain : Window
    {
        xFirewall firewall = new xFirewall();

        public frmMain()
        {
            InitializeComponent();
        }

        private void gridTop_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // firewall.GetFirewallInfo();
            bool isFirewallOn = firewall.GetFirewallEnable();
            togUseFirewall.IsChecked = isFirewallOn;

            togUseFirewall.Checked += togUseFirewall_Checked;
            togUseFirewall.Unchecked += togUseFirewall_Unchecked;
            // var item = firewall.GetFwAllowAppInfo("Agent DVR");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void togUseFirewall_Unchecked(object sender, RoutedEventArgs e)
        {
            firewall.FirewallOnOff(false);
        }

        private void togUseFirewall_Checked(object sender, RoutedEventArgs e)
        {
            firewall.FirewallOnOff(true);
        }

        private void txtUseFirewall_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            togUseFirewall.IsChecked = !togUseFirewall.IsChecked;
        }

        private void btnOpenFirewall_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Start to register
            string command = "wf.msc";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            Process process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo = startInfo;
            process.Start();
            process.StandardInput.Write(command + Environment.NewLine);
            process.StandardInput.Close();

            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            process.Close();
        }
    }
}
