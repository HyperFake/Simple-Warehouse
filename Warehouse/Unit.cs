using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Warehouse
{
    class Unit : INotifyPropertyChanged
    {
        private int id;

        private int key;

        private string state;
        private ObservableCollection<Wood> woods;

        private DateTime startTime;

        private DateTime endTime;

        public Unit(int id, int key, string state, ObservableCollection<Wood> woods, DateTime startTime, DateTime endTime )
        {
            Id = id;
            Key = key;
            State = state;
            Woods = woods;
            StartTime = startTime;
            EndTime = endTime;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// INotify event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged();
            }
        }

        public int Key
        {
            get { return key; }
            set
            {
                key = value;
                NotifyPropertyChanged();
            }
        }

        public string State
        {
            get { return state; }
            set
            {
                state = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Wood> Woods
        {
            get { return woods; }
            set
            {
                woods = value;
                NotifyPropertyChanged();
            }
        }
    }
}
