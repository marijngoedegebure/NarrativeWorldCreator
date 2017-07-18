using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using Semantics.Components;
using System.Collections.ObjectModel;
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

        private bool _hideDueToSingleObjectInstance;
        public bool HideDueToSingleObjectInstance
        {
            get
            {
                return _hideDueToSingleObjectInstance;
            }
            set
            {
                _hideDueToSingleObjectInstance = value;
                OnPropertyChanged("HideDueToSingleObjectInstance");
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

        private bool _required;
        public bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                _required = value;
                OnPropertyChanged("Required");
            }
        }

        private bool _focusable;
        public bool Focusable
        {
            get
            {
                return _focusable;
            }

            set
            {
                _focusable = value;
                OnPropertyChanged("Focusable");
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

        public void Load(NarrativeTimePoint ntp, Relationship r, List<EntikaInstanceSelectionViewModel> objectInstances, EntikaInstance s, EntikaInstance t, List<Models.NarrativeRegionFill.Predicate> predicates)
        {
            this.Relationship = r;
            this.Selected = false;
            this.Focusable = true;
            this.Source = s;
            this.Target = t;

            var relPred = NarrativeTimePoint.GetNewRelationshipPredicate(r, ntp.Location.LocationName);
            if (predicates.Contains(relPred))
                this.Required = true;
            


            ObservableCollection<EntikaInstanceSelectionViewModel> eioc = new ObservableCollection<EntikaInstanceSelectionViewModel>();
            if (this.Source == null)
            {
                foreach (var instanceVM in objectInstances)
                {
                    if (instanceVM.EntikaInstance.TangibleObject.Equals(this.Relationship.Source))
                    {
                        eioc.Add(instanceVM);
                    }
                }
            }
            if (this.Target == null)
            {
                foreach (var instanceVM in objectInstances)
                {
                    foreach (var target in this.Relationship.Targets)
                    {
                        if (instanceVM.EntikaInstance.TangibleObject.Equals(target))
                        {
                            eioc.Add(instanceVM);
                        }
                    }
                }
            }
           
            ObjectInstances = eioc;
        }

        internal RelationshipExtendedViewModel CreateCopy()
        {
            var rel = new RelationshipExtendedViewModel();
            rel.Relationship = this.Relationship;
            rel.Source = this.Source;
            rel.Target = this.Target;
            rel.HideDueToSingleObjectInstance = this.HideDueToSingleObjectInstance;
            rel.Selected = this.Selected;
            rel.Required = this.Required;
            rel.Focusable = this.Focusable;

            var listOC = new ObservableCollection<EntikaInstanceSelectionViewModel>();
            foreach (var instance in this.ObjectInstances)
            {
                var instanceCopy = instance.CreateCopy();
                listOC.Add(instanceCopy);
            }
            rel.ObjectInstances = listOC;
            return rel;
        }
    }
}
