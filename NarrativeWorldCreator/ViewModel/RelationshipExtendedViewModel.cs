using NarrativeWorldCreator.Models.NarrativeRegionFill;
using Semantics.Components;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class RelationshipExtendedViewModel : INotifyPropertyChanged
    {
        private Relationship _relationship;
        public Relationship Relationship
        {
            get
            {
                return _relationship;
            }
            set
            {
                _relationship = value;
                OnPropertyChanged("Relationship");
            }
        }

        private EntikaInstance _source;
        public EntikaInstance Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                OnPropertyChanged("Source");
            }
        }

        private EntikaInstance _target;
        public EntikaInstance Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                OnPropertyChanged("Target");
            }
        }

        private bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
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

        public void Load(Relationship r, List<EntikaInstance> objectInstances, EntikaInstance s, EntikaInstance t)
        {
            this.Relationship = r;
            this.Selected = false;
            this.Source = s;
            this.Target = t;

            ObservableCollection<EntikaInstanceSelectionViewModel> eioc = new ObservableCollection<EntikaInstanceSelectionViewModel>();
            if (this.Source == null)
            {
                foreach (var instance in objectInstances)
                {
                    if (instance.TangibleObject.Equals(this.Relationship.Source))
                    {
                        var objectInstanceVM = new EntikaInstanceSelectionViewModel();
                        objectInstanceVM.EntikaInstance = instance;
                        objectInstanceVM.Selected = false;
                        eioc.Add(objectInstanceVM);
                    }
                }
            }
            if (this.Target == null)
            {
                foreach (var instance in objectInstances)
                {
                    foreach (var target in this.Relationship.Targets)
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
