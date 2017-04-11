using NarrativeWorlds;
using NarrativeWorlds.Models.NarrativeRegionFill;
using Semantics.Components;
using Semantics.Entities;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class RelationInstancingViewModel
    {
        private ObservableCollection<RelationshipInstanceViewModel> _relationshipInstances;
        public ObservableCollection<RelationshipInstanceViewModel> RelationshipInstances
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

        public void Load(List<RelationshipInstance> relationships, List<EntikaInstance> objectInstances)
        {
            // Match all Instanced Objects as potential instance for each relationship
            ObservableCollection<RelationshipInstanceViewModel> rsioc = new ObservableCollection<RelationshipInstanceViewModel>();
            foreach (var rel in relationships)
            {
                var rivm = new RelationshipInstanceViewModel();
                rivm.Load(rel, objectInstances);
                rsioc.Add(rivm);
            }
            RelationshipInstances = rsioc;
        }
    }
}
