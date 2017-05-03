
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class EntikaInstanceSelectionViewModel : INotifyPropertyChanged
    {
        private EntikaInstance _entikaInstance;
        public EntikaInstance EntikaInstance
        {
            get
            {
                return _entikaInstance;
            }

            set
            {
                _entikaInstance = value;
                OnPropertyChanged("EntikaInstance");
            }
        }

        private bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(EntikaInstance instance)
        {
            EntikaInstance = instance;
            Selected = false;
        }
    }
}
