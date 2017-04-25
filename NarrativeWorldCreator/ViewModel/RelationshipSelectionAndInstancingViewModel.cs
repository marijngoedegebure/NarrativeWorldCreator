using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using Semantics.Components;
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
    public class RelationshipSelectionAndInstancingViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<RelationshipExtendedViewModel> _OnRelationships;
        public ObservableCollection<RelationshipExtendedViewModel> OnRelationships
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

        private ObservableCollection<RelationshipExtendedViewModel> _otherRelationships;
        public ObservableCollection<RelationshipExtendedViewModel> OtherRelationships
        {
            get
            {
                return _otherRelationships;
            }
            set
            {
                _otherRelationships = value;
                OnPropertyChanged("OtherRelationships");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(EntikaInstance ei, List<EntikaInstance> ObjectInstances)
        {
            // Fill on relationships
            var onRelationshipsTemp = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in ei.TangibleObject.RelationshipsAsTarget)
            {
                if (Constants.On.Equals(rel.RelationshipType.DefaultName))
                {
                    var tempVM = new RelationshipExtendedViewModel();
                    tempVM.Load(rel, ObjectInstances, null, ei);
                    onRelationshipsTemp.Add(tempVM);
                }
            }
            this.OnRelationships = onRelationshipsTemp;

            // Fill other relationships
            var otherRelationshipsTemp = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in ei.TangibleObject.RelationshipsAsTarget)
            {
                if (Constants.GeometricRelationshipTypes.Contains(rel.RelationshipType.DefaultName))
                {
                    var tempVM = new RelationshipExtendedViewModel();
                    tempVM.Load(rel, ObjectInstances, null, ei);
                    otherRelationshipsTemp.Add(tempVM);
                }
            }
            foreach (var rel in ei.TangibleObject.RelationshipsAsSource)
            {
                if (Constants.GeometricRelationshipTypes.Contains(rel.RelationshipType.DefaultName))
                {
                    var tempVM = new RelationshipExtendedViewModel();
                    tempVM.Load(rel, ObjectInstances, ei, null);
                    otherRelationshipsTemp.Add(tempVM);
                }
            }
            this.OtherRelationships = otherRelationshipsTemp;
        }
    }
}
