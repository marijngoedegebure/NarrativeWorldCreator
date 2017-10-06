using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NarrativeWorldCreator.Models.NarrativeGraph;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models;
using Semantics.Data;
using Semantics.Entities;

namespace NarrativeWorldCreator.ViewModel
{
    class ManualRelationshipsViewModel : INotifyPropertyChanged
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

        private ObservableCollection<OnRelationshipViewModel> _OnRelationships;
        public ObservableCollection<OnRelationshipViewModel> OnRelationships
        {
            get
            {
                return _OnRelationships;
            }
            set
            {
                _OnRelationships = value;
                OnPropertyChanged("OnRelationships");
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

        internal void Load(LocationNode selectedNode, EntikaInstance instanceOfObjectToAdd, List<EntikaInstance> instancedObjects, List<Predicate> list)
        {
            this.CurrentInstance = instanceOfObjectToAdd;

            var OnRelationshipsOC = new ObservableCollection<OnRelationshipViewModel>();
            var OtherRelationshipsOC = new ObservableCollection<OtherRelationshipViewModel>();

            // Load on relationships
            foreach (var rel in instanceOfObjectToAdd.TangibleObject.RelationshipsAsTarget)
            {
                if (!selectedNode.AvailableTangibleObjects.Contains(rel.Source) && !rel.Source.Equals(DatabaseSearch.GetNode<TangibleObject>(Constants.Floor)))
                    continue;
                if (Constants.On.Equals(rel.RelationshipType.DefaultName))
                {
                    foreach(var instance in instancedObjects)
                    {
                        if (rel.Source.Equals(instance.TangibleObject))
                        {
                            var orVM = new OnRelationshipViewModel();
                            orVM.Load(instance, rel);
                            OnRelationshipsOC.Add(orVM);
                        }
                    }
                }
            }

            // Load other relationships, first where instance is source
            foreach (var rel in instanceOfObjectToAdd.TangibleObject.RelationshipsAsTarget)
            {
                if (!selectedNode.AvailableTangibleObjects.Contains(rel.Source))
                    continue;
                if (Constants.OtherRelationshipTypes.Contains(rel.RelationshipType.DefaultName))
                {
                    foreach (var instance in instancedObjects)
                    {
                        if (rel.Source.Equals(instance.TangibleObject))
                        {
                            var otherRVM = new OtherRelationshipViewModel();
                            otherRVM.Load(instance, rel, true);
                            OtherRelationshipsOC.Add(otherRVM);
                        }
                    }
                }
            }

            // And now where instance is target
            foreach (var rel in instanceOfObjectToAdd.TangibleObject.RelationshipsAsSource)
            {
                if (!selectedNode.AvailableTangibleObjects.Contains(rel.Targets[0]))
                    continue;

                if (Constants.OtherRelationshipTypes.Contains(rel.RelationshipType.DefaultName))
                {
                    foreach (var instance in instancedObjects)
                    {
                        if (rel.Source.Equals(instance.TangibleObject))
                        {
                            var otherRVM = new OtherRelationshipViewModel();
                            otherRVM.Load(instance, rel, false);
                            OtherRelationshipsOC.Add(otherRVM);
                        }
                    }
                }
            }

            this.OnRelationships = OnRelationshipsOC;
            this.OtherRelationships = OtherRelationshipsOC;
        }
    }
}
