using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace SetFirewall.Domain
{
    public class DialogsViewModel : ViewModelBase
    {
        public DialogsViewModel()
        {
            //Sample 4
            OpenYesNoDialogCommand = new AnotherCommandImplementation(OpenYesNoDialog);
            AcceptSample4DialogCommand = new AnotherCommandImplementation(AcceptYesNoDialog);
            CancelSample4DialogCommand = new AnotherCommandImplementation(CancelYesNoDialog);
        }

        #region SAMPLE 3

        public ICommand RunDialogCommand => new AnotherCommandImplementation(ExecuteRunDialog);

        public ICommand RunExtendedDialogCommand => new AnotherCommandImplementation(ExecuteRunExtendedDialog);

        private async void ExecuteRunDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new SampleDialog
            {
                DataContext = new SampleDialogViewModel()
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);

            //check the result...
            Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            Debug.WriteLine("You can intercept the closing event, and cancel here.");
        }        

        private async void ExecuteRunExtendedDialog(object o)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new SampleDialog
            {
                DataContext = new SampleDialogViewModel()
            };

            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ExtendedOpenedEventHandler, ExtendedClosingEventHandler);

            //check the result...
            Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));
        }

        private void ExtendedOpenedEventHandler(object sender, DialogOpenedEventArgs eventargs)
            => Debug.WriteLine("You could intercept the open and affect the dialog using eventArgs.Session.");

        private void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter is bool parameter &&
                parameter == false) return;

            //OK, lets cancel the close...
            eventArgs.Cancel();

            //...now, lets update the "session" with some new content!
            eventArgs.Session.UpdateContent(new SampleProgressDialog());
            //note, you can also grab the session when the dialog opens via the DialogOpenedEventHandler

            //lets run a fake operation for 3 seconds then close this baby.
            Task.Delay(TimeSpan.FromSeconds(3))
                .ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region SAMPLE 4

        //pretty much ignore all the stuff provided, and manage everything via custom commands and a binding for .IsOpen
        public ICommand OpenYesNoDialogCommand { get; }
        public ICommand AcceptSample4DialogCommand { get; }
        public ICommand CancelSample4DialogCommand { get; }

        private bool _isYesNoDialogOpen;
        private object? _yesNoContent;

        public bool IsYesNoDialogOpen
        {
            get => _isYesNoDialogOpen;
            set => SetProperty(ref _isYesNoDialogOpen, value);
        }

        public object? YesNoContent
        {
            get => _yesNoContent;
            set => SetProperty(ref _yesNoContent, value);
        }

        private async void OpenYesNoDialog(object obj)
        {
            var YesNoContent = new DialogYesNo();
            IsYesNoDialogOpen = true;
            YesNoContent.m_isOk = false;

            //show the dialog
            var result = await DialogHost.Show(YesNoContent, "RootDialog", ClosingEventHandler);

            //check the result...
            Debug.WriteLine("Dialog was closed, the CommandParameter used to close it was: " + (result ?? "NULL"));

            YesNoContent.m_isOk = (bool)result;
        }

        private void CancelYesNoDialog(object obj) => IsYesNoDialogOpen = false;

        private void AcceptYesNoDialog(object obj)
        {
            //pretend to do something for 3 seconds, then close
            YesNoContent = new SampleProgressDialog();
            Task.Delay(TimeSpan.FromSeconds(3))
                .ContinueWith((t, _) => IsYesNoDialogOpen = false, null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion
    }
}
