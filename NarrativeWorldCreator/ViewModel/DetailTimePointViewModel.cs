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

        private ObservableCollection<GoalViewModel> _predicates;
        public ObservableCollection<GoalViewModel> Predicates
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

        private ObservableCollection<InstancedPredicate> _fillPredicates;
        public ObservableCollection<InstancedPredicate> FillPredicates
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
            ObservableCollection<GoalViewModel> npioc = new ObservableCollection<GoalViewModel>();
            var remainingPredicates = ntp.GetRemainingPredicates();
            var temp = new Predicate[remainingPredicates.Count];
            remainingPredicates.CopyTo(temp);
            var copyOfRemaining = temp.ToList();
            // Determine remaining predicates
            foreach (var pred in ntp.PredicatesFilteredByCurrentLocation)
            {
                if (copyOfRemaining.Contains(pred))
                {
                    copyOfRemaining.Remove(pred);
                    npioc.Add(new GoalViewModel(pred, false));
                }
                else
                {
                    npioc.Add(new GoalViewModel(pred, true));
                }
            }

            this.Predicates = npioc;

            ObservableCollection<InstancedPredicate> fpoc = new ObservableCollection<InstancedPredicate>();
            foreach(var predicate in ntp.PredicatesCausedByInstancedObjectsAndRelations)
            {
                fpoc.Add(predicate);
            }
            this.FillPredicates = fpoc;
        }
    }
}
