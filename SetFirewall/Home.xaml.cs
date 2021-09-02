using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using SetFirewall.Domain;

namespace SetFirewall
{
    public partial class Home
    {
        xFirewall firewall = new xFirewall();
        ObservableCollection<xFwRules> GetAllbounds;
        ObservableCollection<xFwRules> GetInbounds;
        ObservableCollection<xFwRules> GetOutbounds;

        public Home()
        {            
            InitializeComponent();

            DataContext = new DialogsViewModel();
            GetAllbounds = new ObservableCollection<xFwRules>();
            GetInbounds = new ObservableCollection<xFwRules>();
            GetOutbounds = new ObservableCollection<xFwRules>();
        }
            

        private void GitHubButton_OnClick(object sender, RoutedEventArgs e)
        {
            Link.OpenInBrowser("https://github.com/ssallem/SetFirewall");
        }
            

        private void TwitterButton_OnClick(object sender, RoutedEventArgs e)
        { }
            //=> Link.OpenInBrowser("https://twitter.com/James_Willock");

        private void ChatButton_OnClick(object sender, RoutedEventArgs e)
        {
            Link.OpenInBrowser("https://gitter.im/ButchersBoy/MaterialDesignInXamlToolkit");
        }

        private void EmailButton_OnClick(object sender, RoutedEventArgs e)
        {
            Link.OpenInBrowser("mailto://ssallem@nate.com");
        }   

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
            RefreshRules();
        }

        private void RefreshRules()
        {
            GetAllbounds = firewall.GetAllRules();

            GetInbounds = new ObservableCollection<xFwRules>(GetAllbounds.Where(x => x.Direction == NetFwTypeLib.NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN));
            GetOutbounds = new ObservableCollection<xFwRules>(GetAllbounds.Where(x => x.Direction == NetFwTypeLib.NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT));

            this.dataInbound.ItemsSource = GetInbounds;
            this.dataOutbound.ItemsSource = GetOutbounds;
        }

        private async void btnRemove_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IList selectedItems = null;
            DataGrid dataSelected = null;

            if (tabBounds.SelectedIndex == 0)
                dataSelected = dataInbound;
            else
                dataSelected = dataOutbound;

            selectedItems = dataSelected.SelectedItems;

            if (selectedItems == null || selectedItems.Count == 0)
                return;

            var YesNoContent = new DialogYesNo();
            YesNoContent.MyMessage = selectedItems.Count + "건 삭제 하시겠습니까?";

            bool result = (bool)await DialogHost.Show(YesNoContent, "RootDialog", null, null);

            if (result)
            {
                YesNoContent.MyMessage = "실제 방화벽에서 삭제 됩니다." + Environment.NewLine + "계속 하시겠습니까?";

                if ((bool)await DialogHost.Show(YesNoContent, "RootDialog", null, null) == false)
                {
                    return;
                }

                // 기존 고급 방화벽 정보 백업
                string wfwBackup = String.Format("netsh advfirewall export \"{0}\"", 
                                   System.Environment.CurrentDirectory + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".wfw");

                bool isOk = xCommon.ExcuteCmd(wfwBackup);
                var msgDialog = new SampleMessageDialog();
                msgDialog.Message.Text = "Firewall Back up is Ok : " + isOk.ToString() + Environment.NewLine + wfwBackup;
                await DialogHost.Show(msgDialog, "RootDialog", null, null);

                //foreach (var item in selectedItems)
                //{
                //    DataRowView row = (DataRowView)item;
                //    GetInbounds.Remove((clsInBound)row);
                //}

                    //dataInbound.Items.Remove(selectedItems);
            }
        }

        private void btnAdd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnStop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IList selectedItems = null;
            DataGrid dataSelected = null;

            if (tabBounds.SelectedIndex == 0)
                dataSelected = dataInbound;                
            else
                dataSelected = dataOutbound;

            selectedItems = dataSelected.SelectedItems;

            if (selectedItems == null || selectedItems.Count == 0)
                return;

            foreach (var item in selectedItems)
            {
                xFwRules ruleItem = (xFwRules)item;
                ruleItem.IsEnabled = false;
                ruleItem.EnableName = "중지";
                firewall.ApplyRules(xFirewall.RuleString.DEL_PROGRAM, ruleItem.RuleName, ruleItem.DirectionName, "no");
            }
            dataSelected.Items.Refresh();
        }

        private void btnStart_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IList selectedItems = null;
            DataGrid dataSelected = null;

            if (tabBounds.SelectedIndex == 0)
                dataSelected = dataInbound;
            else
                dataSelected = dataOutbound;

            selectedItems = dataSelected.SelectedItems;

            if (selectedItems == null || selectedItems.Count == 0)
                return;

            foreach (var item in selectedItems)
            {
                xFwRules ruleItem = (xFwRules)item;                
                ruleItem.IsEnabled = true;
                ruleItem.EnableName = "사용";

                firewall.ApplyRules(xFirewall.RuleString.UPDATE_ENABLE, ruleItem.RuleName, ruleItem.DirectionName, "yes");
            }
            dataSelected.Items.Refresh();
        }
    }
}
