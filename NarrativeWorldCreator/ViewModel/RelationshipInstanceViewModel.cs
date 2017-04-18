using NarrativeWorldCreator.Models.NarrativeRegionFill;
using SharpDX.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace NarrativeWorldCreator.ViewModel
{
    public class RelationshipInstanceViewModel : INotifyPropertyChanged
    {
        private RelationshipInstance _relationshipInstance;
        public RelationshipInstance RelationshipInstance
        {
            get
            {
                return _relationshipInstance;
            }

            set
            {
                _relationshipInstance = value;
                OnPropertyChanged("RelationshipInstance");
            }
        }

        private ObservableCollection<EntikaInstanceSelectionViewModel> _objectInstances;
        public ObservableCollection<EntikaInstanceSelectionViewModel> ObjectInstances
        {
            get
            {
                return _objectInstances;
            }
            set
            {
                    _objectInstances = value;
                OnPropertyChanged("ObjectInstances");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(RelationshipInstance relationship, List<EntikaInstance> objectInstances)
        {
            RelationshipInstance = relationship;
            // Filter object instances using relationshipInstance

            ObservableCollection<EntikaInstanceSelectionViewModel> eioc = new ObservableCollection<EntikaInstanceSelectionViewModel>();
            if (relationship.Source == null)
            {
                foreach (var instance in objectInstances)
                {
                    if (instance.TangibleObject.Equals(relationship.BaseRelationship.Source))
                    {
                        var objectInstanceVM = new EntikaInstanceSelectionViewModel();
                        objectInstanceVM.EntikaInstance = instance;
                        objectInstanceVM.Selected = false;
                        eioc.Add(objectInstanceVM);
                    }
                }
            }
            else
            {
                foreach (var instance in objectInstances)
                {
                    foreach(var target in relationship.BaseRelationship.Targets)
                    {
                        if (instance.TangibleObject.Equals(target))
                        {
                            var objectInstanceVM = new EntikaInstanceSelectionViewModel();
                            objectInstanceVM.EntikaInstance = instance;
                            objectInstanceVM.Selected = false;
                            eioc.Add(objectInstanceVM);
                        }
                    }
                }
            }

            ObjectInstances = eioc;
        }
    }
}