using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class DetailTimePointViewModel : INotifyPropertyChanged
    {

        private NarrativeTimePoint _narrativeTimePoint;
        public NarrativeTimePoint NarrativeTimePoint
        {
            get
            {
                return _narrativeTimePoint;
            }
            set
            {
                _narrativeTimePoint = value;
                OnPropertyChanged("NarrativeTimePoint");
            }
        }

        private ObservableCollection<NarrativePredicateInstance> _narrativePredicateInstances;
        public ObservableCollection<NarrativePredicateInstance> NarrativePredicateInstances
        {
            get
            {
                return _narrativePredicateInstances;
            }
            set
            {
                _narrativePredicateInstances = value;
                OnPropertyChanged("NarrativePredicateInstances");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        internal void LoadObjects(LocationNode selectedNode, NarrativeTimePoint ntp)
        {
            this.NarrativeTimePoint = ntp;
            ObservableCollection<NarrativePredicateInstance> npioc = new ObservableCollection<NarrativePredicateInstance>();

            foreach (var pi in ntp.PredicatesFilteredByCurrentLocation)
            {
                npioc.Add(pi);
            }
            this.NarrativePredicateInstances = npioc;
        }
    }
}
