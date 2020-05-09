using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace InterruptionReport.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public BaseViewModel(ContentPage view)
        {
            BaseContent = view;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }

        protected bool SetProperty<T>(ref T storage, T value, bool checkEquality = false, [CallerMemberName]string propertyName = null)
        {
            if (checkEquality && EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        public async Task SingleRun(Func<Task> operation)
        {
            object _lock = new object();
            lock (_lock)
            {
                if (CommandInitiated)
                    return;

                CommandInitiated = true;
            }

            try
            {
                await operation();
            }
            catch (Exception e)
            {
                WriteExceptionToConsole(e);
            }
            CommandInitiated = false;
        }
        public async Task<bool> AskForPermission<T>(T permission) where T : BasePermission
        {
            try
            {
                var status = await permission.CheckStatusAsync();
                if (status != PermissionStatus.Granted)
                {
                    status = await permission.RequestAsync();
                }


                return status == PermissionStatus.Granted;

            }
            catch (Exception ex)
            {
                await BaseContent.DisplayAlert("Error:", ex.Message, "OK");
                return false;
            }
        }
        public void WriteExceptionToConsole(Exception e, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callerfile = null, [CallerMemberName] string caller = null)
        {
            Debug.WriteLine("Exception thrown in: " + callerfile + " at position- Line No:" + lineNumber + ", Caller Member:" + caller + ", Message:" + e.Message);
            Debug.WriteLine(e.StackTrace);
        }

        #region Property
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (value)
                        UserDialogs.Instance.ShowLoading();
                    else
                        UserDialogs.Instance.HideLoading();
                });
            }
        }

        public ContentPage BaseContent { get; set; }
        public bool CommandInitiated { get; set; }
        #endregion
    }
}
