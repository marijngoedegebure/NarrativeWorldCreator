using NarrativeWorlds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    public class SelectedObjectDetailViewModel : INotifyPropertyChanged
    {

        private NarrativeShape _selectedInstancedEntikaObject;
        public NarrativeShape SelectedInstancedEntikaObject
        {
            get
            {
                return _selectedInstancedEntikaObject;
            }
            set
            {
                _selectedInstancedEntikaObject = value;
                OnPropertyChanged("SelectedInstancedEntikaObject");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        internal void ChangeSelectedObject(NarrativeShape newlySelected)
        {
            SelectedInstancedEntikaObject = newlySelected;
        }
    }
}
