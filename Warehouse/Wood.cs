using System;
using System.ComponentModel;

namespace Warehouse
{
    class Wood : INotifyPropertyChanged
    {
        private int id;
        private double height;
        private double width;
        private double depth;
        private int packages;
        private int singles;
        private string type;
        private string note;

        public event PropertyChangedEventHandler PropertyChanged;

        public Wood(int id, double height, double width, double depth,
                    int packages, int singles, string type = "", string note = "")
        {
            Id = id;
            Height = height;
            Width = width;
            Depth = depth;
            Packages = packages;
            Singles = singles;
            Type = type;
            Note = note;
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

        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                NotifyPropertyChanged();
            }
        }

        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                NotifyPropertyChanged();
            }
        }

        public double Depth
        {
            get { return depth; }
            set
            {
                depth = value;
                NotifyPropertyChanged();
            }
        }

        public int Packages
        {
            get { return packages; }
            set
            {
                packages = value;
                NotifyPropertyChanged();
            }
        }

        public int Singles
        {
            get { return singles; }
            set
            {
                singles = value;
                NotifyPropertyChanged();
            }
        }

        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                NotifyPropertyChanged();
            }
        }

        public string Note
        {
            get { return note; }
            set
            {
                note = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// INotify event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
