using MaterialDesignThemes.Wpf;
using System.Windows.Controls;

namespace SetFirewall.Domain
{
    /// <summary>
    /// Interaction logic for SampleDialog.xaml
    /// </summary>
    public partial class DialogYesNo : UserControl
    {
        public string MyMessage 
        {
            get { return txtMessage.Text; }
            set 
            {
                this.txtMessage.Text = value; 
            } 
        }

        public bool m_isOk = false;

        public DialogYesNo()
        {
            InitializeComponent();
        }

        private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            m_isOk = (bool)eventArgs.Parameter;
        }
    }
}
