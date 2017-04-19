using NarrativeWorldCreator.MetricEngines;
using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Solvers;
using Semantics.Components;
using Semantics.Data;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class FillDetailViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<EntikaInstanceValued> _entikaInstancesValued;
        public ObservableCollection<EntikaInstanceValued> EntikaInstancesValued
        {
            get
            {
                return _entikaInstancesValued;
            }
            set
            {
                _entikaInstancesValued = value;
                OnPropertyChanged("EntikaInstancesValued");
            }
        }

        private ObservableCollection<RelationshipInstanceEnergyViewModel> _relationshipInstances;
        public ObservableCollection<RelationshipInstanceEnergyViewModel> RelationshipInstances
        {
            get
            {
                return _relationshipInstances;
            }
            set
            {
                _relationshipInstances = value;
                OnPropertyChanged("RelationshipInstances");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(NarrativeTimePoint ntp)
        {
            ObservableCollection<EntikaInstanceValued> eioc = new ObservableCollection<EntikaInstanceValued>();
            var listOfinstances = EntikaInstanceMetricEngine.GetDecorationOrderingEI(ntp);
            foreach (var instance in listOfinstances)
            {
                eioc.Add(instance);
            }
            EntikaInstancesValued = eioc;

            ObservableCollection<RelationshipInstanceEnergyViewModel> rioc = new ObservableCollection<RelationshipInstanceEnergyViewModel>();
            foreach (var relation in ntp.InstancedRelations)
            {
                var relationshipEnergyVM = new RelationshipInstanceEnergyViewModel();
                relationshipEnergyVM.Load(relation);
                rioc.Add(relationshipEnergyVM);
            }
            RelationshipInstances = rioc;
        }
    }
}
