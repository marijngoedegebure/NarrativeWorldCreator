using NarrativeWorldCreator.Models.NarrativeGraph;
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
    internal class DeltaListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<InstanceDelta> _instanceDeltas;
        public ObservableCollection<InstanceDelta> InstanceDeltas
        {
            get
            {
                return _instanceDeltas;
            }
            set
            {
                _instanceDeltas = value;
                OnPropertyChanged("InstanceDeltas");
            }
        }

        private ObservableCollection<RelationshipDelta> _relationshipDeltas;
        public ObservableCollection<RelationshipDelta> RelationshipDeltas
        {
            get
            {
                return _relationshipDeltas;
            }
            set
            {
                _relationshipDeltas = value;
                OnPropertyChanged("RelationshipDeltas");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        internal void Load(LocationNode selectedNode, int TimePoint)
        {
            var idoc = new ObservableCollection<InstanceDelta>();
            foreach (var instanceDelta in selectedNode.TimePoints[TimePoint].InstanceDeltas)
            {
                idoc.Add(instanceDelta);
            }
            InstanceDeltas = idoc;

            var iroc = new ObservableCollection<RelationshipDelta>();
            foreach (var relationshipDelta in selectedNode.TimePoints[TimePoint].RelationshipDeltas)
            {
                iroc.Add(relationshipDelta);
            }
            RelationshipDeltas = iroc;
        }
    }
}
