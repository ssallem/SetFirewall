using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SetFirewall.Domain
{
    /// <summary>
    /// Interaction logic for SampleDialog.xaml
    /// </summary>
    public partial class AddFwRuleDialog : UserControl
    {
        /// <summary>
        /// Property 바인딩 생성
        /// </summary>
        public static readonly DependencyProperty ProtocolProperty =
            DependencyProperty.Register("ProtocolType", typeof(string), typeof(AddFwRuleDialog), new UIPropertyMetadata(string.Empty));

        public string ProtocolType
        {
            get { return (string)this.GetValue(ProtocolProperty); }
            set { this.SetValue(ProtocolProperty, value); }
        }

        /// <summary>
        /// ValidOK 바인딩 생성
        /// </summary>
        public static readonly DependencyProperty ValidOKProperty =
            DependencyProperty.Register("ValidOK", typeof(bool), typeof(AddFwRuleDialog), new UIPropertyMetadata(true));

        public bool ValidOK
        {
            get { return (bool)this.GetValue(ValidOKProperty); }
            set { this.SetValue(ValidOKProperty, value); }
        }

        public AddFwRuleDialog()
        {
            InitializeComponent();
            DataContext = new FieldsViewModel();
        }

        private void rdbProgram_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            grdProtocol.Visibility = System.Windows.Visibility.Collapsed;
            txtFilePath.Visibility = System.Windows.Visibility.Visible;
            txtPort.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void rdbPort_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            grdProtocol.Visibility = System.Windows.Visibility.Visible;
            txtFilePath.Visibility = System.Windows.Visibility.Hidden;
            txtPort.Visibility = System.Windows.Visibility.Visible;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            rdbProgram.IsChecked = true;
            rdbProtocolAll.IsChecked = true;
        }

        private void rdbProtocol_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rdbMe = sender as RadioButton;
            ProtocolType = rdbMe.Content.ToString();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // validation            
            ValidOK = false;
            if (txtRuleName.Text.Length == 0) return;

            if ((bool)rdbProgram.IsChecked)
            {
                if (txtFilePath.Text.Length == 0) return;
                if (!File.Exists(txtFilePath.Text)) return;
            }
            else
            {
                if (txtPort.Text.Length == 0) return;
            }
            ValidOK = true;
        }
    }
}
