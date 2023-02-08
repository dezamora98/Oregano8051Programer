using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AlarmViewProyect
{
    public abstract class ViewModelBase : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanging([CallerMemberName] string propertyName = "")
        {
            if ((this.PropertyChanging == null))
                throw new ArgumentNullException(null, "PropertyChanging is null");
            PropertyChanging(this, new PropertyChangingEventArgs(propertyName));


        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
