using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using Semantics.Data;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    class AutomatedRelationshipSelectionViewModel : INotifyPropertyChanged
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

        private OnRelationshipViewModel _OnRelationship;
        public OnRelationshipViewModel OnRelationship
        {
            get
            {
                return _OnRelationship;
            }
            set
            {
                _OnRelationship = value;
                OnPropertyChanged("OnRelationship");
            }
        }

        private ObservableCollection<OtherRelationshipViewModel> _OtherRelationships;
        public ObservableCollection<OtherRelationshipViewModel> OtherRelationships
        {
            get
            {
                return _OtherRelationships;
            }
            set
            {
                _OtherRelationships = value;
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
