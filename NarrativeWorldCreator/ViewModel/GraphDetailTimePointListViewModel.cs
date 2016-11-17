﻿using NarrativeWorlds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    public class GraphDetailTimePointListViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<NarrativeTimePoint> _narrativeTimePoints;
        public ObservableCollection<NarrativeTimePoint> NarrativeTimePoints
        {
            get
            {
                return _narrativeTimePoints;
            }
            set
            {
                _narrativeTimePoints = value;
                OnPropertyChanged("NarrativeTimePoints");
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
