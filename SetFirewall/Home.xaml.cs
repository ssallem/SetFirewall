using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NetFwTypeLib;
using SetFirewall.Domain;

namespace SetFirewall
{
    public partial class Home
    {
        /// <summary>방화벽 개체</summary>
        xFirewall firewall = new xFirewall();

        /// <summary>전체 방화벽 Rules</summary>
        ObservableCollection<xFwRules> GetAllbounds;
        /// <summary>인바운드 방화벽 Rules</summary>
        ObservableCollection<xFwRules> GetInbounds;
        /// <summary>아웃바운드 방화벽 Rules</summary>
        ObservableCollection<xFwRules> GetOutbounds;

        /// <summary>메세지를 메인 폼에 전달한다.</summary>
        public delegate void CallMessageToForm(string message);
        public static event CallMessageToForm CallMsgToForm;

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

        private void ChatButton_OnClick(object sender, RoutedEventArgs e)
        {
            Link.OpenInBrowser("https://gitter.im/ButchersBoy/MaterialDesignInXamlToolkit");
        }

        private void EmailButton_OnClick(object sender, RoutedEventArgs e)
        {
            Link.OpenInBrowser("mailto://ssallem@nate.com");
        }   

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

        /// <summary>
        /// 방화벽 Rules 새로고침
        /// </summary>
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

            CallMsgToForm("방화벽 삭제");

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
                string nowDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string wfwBackup = String.Format("netsh advfirewall export \"{0}\"", 
                                   System.Environment.CurrentDirectory + @"\" + nowDate + ".wfw");

                bool isOk = xCommon.ExcuteCmd(wfwBackup);
                var msgDialog = new SampleMessageDialog();
                msgDialog.Message.Text = "방화벽 설정 백업 성공 여부 : " + isOk.ToString() + Environment.NewLine +
                                         System.Environment.CurrentDirectory + @"\" + nowDate + ".wfw";

                await DialogHost.Show(msgDialog, "RootDialog", null, null);

                for (int i = selectedItems.Count - 1; i >= 0; i--)
                {
                    xFwRules ruleItem = (xFwRules)selectedItems[i];
                    
                    if (String.IsNullOrEmpty(ruleItem.RuleName))
                        continue;

                    if (String.IsNullOrEmpty(ruleItem.RuleProgram))
                    {
                        if (String.IsNullOrEmpty(ruleItem.LocalPort))
                            firewall.ApplyRules(xFirewall.RuleString.DEL_NAME, ruleItem.RuleName, ruleItem.DirectionName);
                        else
                            firewall.ApplyRules(xFirewall.RuleString.DEL_PORT, ruleItem.RuleName, ruleItem.DirectionName, ruleItem.Protocol, ruleItem.LocalPort);
                    }
                    else
                    {
                        firewall.ApplyRules(xFirewall.RuleString.DEL_PROGRAM, ruleItem.RuleName, ruleItem.DirectionName, ruleItem.RuleProgram);
                    }

                    // Binding된 Collection에 Add 해야한다.
                    if (tabBounds.SelectedIndex == 0)
                    {
                        GetInbounds.Remove(ruleItem);
                    }
                    else
                    {
                        GetOutbounds.Remove(ruleItem);
                    }
                }
                dataSelected.Items.Refresh();
            }
        }

        private async void btnAdd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CallMsgToForm("방화벽 추가");
            var AddRuleDialog = new AddFwRuleDialog();

            object result = await DialogHost.Show(AddRuleDialog, "RootDialog", null, DialogHost_DialogClosing);

            if (result.GetType() == typeof(Boolean))
            {
                // 취소 버튼 클릭
            }
            else
            {
                // Validation 체크는 AddFwRuleDialog 에서 완료.
                Object[] objResult = (Object[])result;

                if (objResult.Length != 6)
                {
                    return;
                }
                string ruleName = objResult[0].ToString();
                bool isProgram = (bool)objResult[1];
                string programPath = objResult[2].ToString();
                string protocolType = objResult[3].ToString();
                string portName = objResult[4].ToString();
                string directionName = "out";

                DataGrid dataSelected = null;

                if (tabBounds.SelectedIndex == 0)
                {
                    dataSelected = dataInbound;
                    directionName = "in";
                }
                else
                {
                    dataSelected = dataOutbound;
                }                     

                xFwRules ruleItem = new xFwRules();
                ruleItem.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                ruleItem.ActionName = xCommon.GetRuleActionName(ruleItem.Action);
                ruleItem.DirectionName = directionName;

                if (ruleItem.DirectionName == "in")
                    ruleItem.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                else
                    ruleItem.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;

                ruleItem.IsEnabled = true;
                ruleItem.EnableName = "사용";
                ruleItem.LocalPort = portName;
                ruleItem.Protocol = protocolType;
                ruleItem.RuleName = ruleName;
                ruleItem.RuleProgram = programPath;

                if (isProgram)
                {
                    firewall.ApplyRules(xFirewall.RuleString.ADD_PROGRAM, ruleName, directionName, programPath);
                }
                else
                {
                    firewall.ApplyRules(xFirewall.RuleString.ADD_PORT, ruleName, directionName, protocolType, portName);
                }

                // Binding된 Collection에 Add 해야한다.
                if (tabBounds.SelectedIndex == 0)
                {
                    GetInbounds.Add(ruleItem);
                }
                else
                {
                    GetOutbounds.Add(ruleItem);
                }
                dataSelected.Items.Refresh();
            }
        }

        /// <summary>
        /// 확인 버튼 클릭시 조건 불충분하면 Dialog창 Close Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void DialogHost_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter == null) return;
            if (eventArgs.Parameter.GetType() == typeof(Boolean)) return;

            Object[] objResult = (Object[])eventArgs.Parameter;
            bool validOk = (bool)objResult[5];

            if (!validOk)
            {
                eventArgs.Cancel();
            }
        }

        private void btnStop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CallMsgToForm("방화벽 Rule 중지");

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
                firewall.ApplyRules(xFirewall.RuleString.UPDATE_ENABLE, ruleItem.RuleName, ruleItem.DirectionName, "no");
            }
            dataSelected.Items.Refresh();
        }

        private void btnStart_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CallMsgToForm("방화벽 Rule 시작");

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

        private void OpenFwDetailButton_OnClick(object sender, RoutedEventArgs e)
        {
            CallMsgToForm("고급 방화벽 설정 열기");
            xCommon.ExcuteCmd("wf.msc");
        }
    }
}
