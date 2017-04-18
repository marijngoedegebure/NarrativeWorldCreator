using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class Vector3ViewModel : INotifyPropertyChanged
    {
        private double _x;
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                    _x = value;
                OnPropertyChanged("X");
            }
        }

        private double _y;
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                OnPropertyChanged("Y");
            }
        }

        private double _z;
        public double Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
                OnPropertyChanged("Z");
            }
        }

        private Vector3 _originalVector;
        public Vector3 OriginalVector
        {
            get
            {
                return _originalVector;
            }
            set
            {
                _originalVector = value;
                OnPropertyChanged("OriginalVector");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(Vector3 option)
        {
            this.X = Math.Round(option.X, 2);
            this.Y = Math.Round(option.Y, 2);
            this.Z = Math.Round(option.Z, 2);
            this.OriginalVector = option;
        }
    }
}
