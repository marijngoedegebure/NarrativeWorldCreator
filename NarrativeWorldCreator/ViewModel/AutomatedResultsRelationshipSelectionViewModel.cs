using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    class AutomatedResultsRelationshipSelectionViewModel : INotifyPropertyChanged
    {
        private EntikaInstance _currentInstance;
        public EntikaInstance CurrentInstance
        {
            get
            {
                return _currentInstance;
            }
            set
            {
                _currentInstance = value;
                OnPropertyChanged("CurrentInstance");
            }
        }

        private ObservableCollection<AutomatedRelationshipSelectionViewModel> _results;
        public ObservableCollection<AutomatedRelationshipSelectionViewModel> Results
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;
                OnPropertyChanged("OtherRelationships");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
