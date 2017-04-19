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

        private ObservableCollection<Predicate> _predicates;
        public ObservableCollection<Predicate> Predicates
        {
            get
            {
                return _predicates;
            }
            set
            {
                _predicates = value;
                OnPropertyChanged("Predicates");
            }
        }

        private ObservableCollection<Predicate> _fillPredicates;
        public ObservableCollection<Predicate> FillPredicates
        {
            get
            {
                return _fillPredicates;
            }
            set
            {
                _fillPredicates = value;
                OnPropertyChanged("FillPredicates");
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
            ObservableCollection<Predicate> npioc = new ObservableCollection<Predicate>();

            foreach (var pi in ntp.PredicatesFilteredByCurrentLocation)
            {
                npioc.Add(pi);
            }
            this.Predicates = npioc;

            ObservableCollection<Predicate> fpoc = new ObservableCollection<Predicate>();
            foreach(var predicate in ntp.PredicatesCausedByInstancedObjectsAndRelations)
            {
                fpoc.Add(predicate);
            }
            this.FillPredicates = fpoc;
        }
    }
}
