using System.ComponentModel;
// This file is used to define the BaseViewModel class, which implements the INotifyPropertyChanged interface.
// The BaseViewModel class is used as a base class for all view models in the application. It implements the INotifyPropertyChanged interface, which is used to notify the view when a property changes.
// The BaseViewModel class defines a SetProperty method that is used to set the value of a property and raise the PropertyChanged event if the value has changed.
// The BaseViewModel class also defines an OnPropertyChanged method that is used to raise the PropertyChanged event when a property changes. This is the most important bit for the MVVM pattern.
// Simple explanation: The BaseViewModel class is used as a base class for all view models in the application. It implements the INotifyPropertyChanged interface to notify the view when a property changes.
namespace PrototypeForAnkiEsque.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
