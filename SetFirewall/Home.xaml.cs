using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using SetFirewall.Domain;

namespace SetFirewall
{
    public partial class Home
    {
        xFirewall firewall = new xFirewall();

        public Home()
        {
            DataContext = new DialogsViewModel();
            InitializeComponent();
        }
            

        private void GitHubButton_OnClick(object sender, RoutedEventArgs e)
        { }
            //=> Link.OpenInBrowser(ConfigurationManager.AppSettings["GitHub"]);

        private void TwitterButton_OnClick(object sender, RoutedEventArgs e)
        { }
            //=> Link.OpenInBrowser("https://twitter.com/James_Willock");

        private void ChatButton_OnClick(object sender, RoutedEventArgs e)
        { }
            //=> Link.OpenInBrowser("https://gitter.im/ButchersBoy/MaterialDesignInXamlToolkit");

        private void EmailButton_OnClick(object sender, RoutedEventArgs e)
        { }
            //=> Link.OpenInBrowser("mailto://james@dragablz.net");

        private void DonateButton_OnClick(object sender, RoutedEventArgs e)
        { }
        //=> Link.OpenInBrowser("https://opencollective.com/materialdesigninxaml");

        private void txtUseFirewall_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            togUseFirewall.IsChecked = !togUseFirewall.IsChecked;
        }

        private void togUseFirewall_Unchecked(object sender, RoutedEventArgs e)
        {
            firewall.FirewallOnOff(false);
        }

        private void togUseFirewall_Checked(object sender, RoutedEventArgs e)
        {
            firewall.FirewallOnOff(true);
        }

        private void ucMain_Loaded(object sender, RoutedEventArgs e)
        {
            List<clsInBound> list = new List<clsInBound>();

            for (int i = 1; i <= 5; i++)
            {
                clsInBound clsInBound = new clsInBound();
                clsInBound.In_Name = "name" + i.ToString();
                clsInBound.In_Program = "program" + i.ToString();
                clsInBound.In_Protocol = "protocol" + i.ToString();
                clsInBound.In_LocalPort = "local" + i.ToString();
                clsInBound.In_RemotePort = "remote" + i.ToString();
                list.Add(clsInBound);
            }

            this.dataInbound.ItemsSource = list;
        }
    }

    public class clsInBound
    {
        public string In_Name {  get; set; }
        public string In_Program { get; set; }
        public string In_Protocol { get; set; }        
        public string In_LocalPort { get; set; }
        public string In_RemotePort { get; set; }
    }
}
