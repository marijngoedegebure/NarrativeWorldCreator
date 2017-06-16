using NarrativeWorldCreator.Models.Metrics;
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
    public class EntikaInstanceValuedPredicate : INotifyPropertyChanged
    {
        private EntikaInstanceValued _entikaInstanceValued;
        public EntikaInstanceValued EntikaInstanceValued
        {
            get
            {
                return _entikaInstanceValued;
            }
            set
            {
                _entikaInstanceValued = value;
                OnPropertyChanged("EntikaInstanceValued");
            }
        }

        private ObservableCollection<InstancedPredicate> _instancedPredicates;
        public ObservableCollection<InstancedPredicate> InstancedPredicates
        {
            get
            {
                return _instancedPredicates;
            }
            set
            {
                _instancedPredicates = value;
                OnPropertyChanged("InstancedPredicates");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public EntikaInstanceValuedPredicate(EntikaInstanceValued eiv, NarrativeTimePoint ntp)
        {
            this.EntikaInstanceValued = eiv;
            // Retrieve predicates for current instance
            var ipoc = new ObservableCollection<InstancedPredicate>();
            var instPredicates = ntp.GetPredicatesOfInstance(eiv.EntikaInstance);
            foreach (var inst in instPredicates)
            {
                ipoc.Add(inst);
            }
            InstancedPredicates = ipoc;
        }
    }
}
