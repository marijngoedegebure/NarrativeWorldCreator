using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
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
        private ObservableCollection<RelationshipExtendedViewModel> _OnRelationshipsMultiple;
        public ObservableCollection<RelationshipExtendedViewModel> OnRelationshipsMultiple
        {
            get
            {
                return _OnRelationshipsMultiple;
            }
            set
            {
                _OnRelationshipsMultiple = value;
                OnPropertyChanged("OnRelationshipsMultiple");
            }
        }

        private ObservableCollection<RelationshipExtendedViewModel> _otherRelationshipsMultiple;
        public ObservableCollection<RelationshipExtendedViewModel> OtherRelationshipsMultiple
        {
            get
            {
                return _otherRelationshipsMultiple;
            }
            set
            {
                _otherRelationshipsMultiple = value;
                OnPropertyChanged("OtherRelationshipsMultiple");
            }
        }

        private ObservableCollection<RelationshipExtendedViewModel> _OnRelationshipsSingle;
        public ObservableCollection<RelationshipExtendedViewModel> OnRelationshipsSingle
        {
            get
            {
                return _OnRelationshipsSingle;
            }
            set
            {
                _OnRelationshipsSingle = value;
                OnPropertyChanged("OnRelationshipsSingle");
            }
        }

        private ObservableCollection<RelationshipExtendedViewModel> _otherRelationshipsSingle;
        public ObservableCollection<RelationshipExtendedViewModel> OtherRelationshipsSingle
        {
            get
            {
                return _otherRelationshipsSingle;
            }
            set
            {
                _otherRelationshipsSingle = value;
                OnPropertyChanged("OtherRelationshipsSingle");
            }
        }

        private ObservableCollection<RelationshipExtendedViewModel> _OnRelationshipsNone;
        public ObservableCollection<RelationshipExtendedViewModel> OnRelationshipsNone
        {
            get
            {
                return _OnRelationshipsNone;
            }
            set
            {
                _OnRelationshipsNone = value;
                OnPropertyChanged("OnRelationshipsNone");
            }
        }

        private ObservableCollection<RelationshipExtendedViewModel> _otherRelationshipsNone;
        public ObservableCollection<RelationshipExtendedViewModel> OtherRelationshipsNone
        {
            get
            {
                return _otherRelationshipsNone;
            }
            set
            {
                _otherRelationshipsNone = value;
                OnPropertyChanged("OtherRelationshipsNone");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(NarrativeTimePoint ntp, EntikaInstance ei, List<EntikaInstance> ObjectInstances, List<Models.NarrativeRegionFill.Predicate> predicates)
        {
            // Setup Entika instance VMs
            List<EntikaInstanceSelectionViewModel> eioc = new List<EntikaInstanceSelectionViewModel>();
            foreach (var instance in ObjectInstances)
            {
                var objectInstanceVM = new EntikaInstanceSelectionViewModel();
                objectInstanceVM.EntikaInstance = instance;
                objectInstanceVM.Selected = false;
                eioc.Add(objectInstanceVM);
            }

            // Fill on relationships
            var onRelationshipsMultipleTemp = new ObservableCollection<RelationshipExtendedViewModel>();
            var onRelationshipsSingleTemp = new ObservableCollection<RelationshipExtendedViewModel>();
            var onRelationshipsNoneTemp = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in ei.TangibleObject.RelationshipsAsTarget)
            {
                if (Constants.On.Equals(rel.RelationshipType.DefaultName))
                {
                    var tempVM = new RelationshipExtendedViewModel();
                    tempVM.Load(ntp, rel, eioc, null, ei, predicates);
                    if (tempVM.ObjectInstances.Count == 0)
                        onRelationshipsNoneTemp.Add(tempVM);
                    else if (tempVM.ObjectInstances.Count == 1)
                    {
                        tempVM.ObjectInstances[0].Selected = true;
                        onRelationshipsSingleTemp.Add(tempVM);
                    }
                    else
                        onRelationshipsMultipleTemp.Add(tempVM);
                }
            }
            this.OnRelationshipsMultiple = onRelationshipsMultipleTemp;
            this.OnRelationshipsSingle = onRelationshipsSingleTemp;
            this.OnRelationshipsNone = onRelationshipsNoneTemp;

            // Fill other relationships
            var otherRelationshipsMultipleTemp = new ObservableCollection<RelationshipExtendedViewModel>();
            var otherRelationshipsSingleTemp = new ObservableCollection<RelationshipExtendedViewModel>();
            var otherRelationshipsNoneTemp = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in ei.TangibleObject.RelationshipsAsTarget)
            {
                if (!ntp.AvailableTangibleObjects.Contains(rel.Source))
                    continue;
                if (Constants.OtherRelationshipTypes.Contains(rel.RelationshipType.DefaultName))
                {
                    var tempVM = new RelationshipExtendedViewModel();
                    tempVM.Load(ntp, rel, eioc, null, ei, predicates);
                    if (tempVM.ObjectInstances.Count == 0)
                        otherRelationshipsNoneTemp.Add(tempVM);
                    else if (tempVM.ObjectInstances.Count == 1)
                    {
                        tempVM.ObjectInstances[0].Selected = true;
                        otherRelationshipsSingleTemp.Add(tempVM);
                    }
                    else
                        otherRelationshipsMultipleTemp.Add(tempVM);
                }
            }
            foreach (var rel in ei.TangibleObject.RelationshipsAsSource)
            {
                if (!ntp.AvailableTangibleObjects.Contains(rel.Targets[0]))
                    continue;

                if (Constants.OtherRelationshipTypes.Contains(rel.RelationshipType.DefaultName))
                {
                    var tempVM = new RelationshipExtendedViewModel();
                    tempVM.Load(ntp, rel, eioc, ei, null, predicates);
                    if (tempVM.ObjectInstances.Count == 0)
                        otherRelationshipsNoneTemp.Add(tempVM);
                    else if (tempVM.ObjectInstances.Count == 1)
                    {
                        tempVM.ObjectInstances[0].Selected = true;
                        otherRelationshipsSingleTemp.Add(tempVM);
                    }
                    else
                        otherRelationshipsMultipleTemp.Add(tempVM);
                }
            }
            this.OtherRelationshipsMultiple = otherRelationshipsMultipleTemp;
            this.OtherRelationshipsSingle = otherRelationshipsSingleTemp;
            this.OtherRelationshipsNone = otherRelationshipsNoneTemp;
        }

        internal RelationshipSelectionAndInstancingViewModel CreateCopy()
        {
            var riVM = new RelationshipSelectionAndInstancingViewModel();
            var relOC = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in this.OnRelationshipsMultiple)
            {
                var relCopy = rel.CreateCopy();
                relOC.Add(relCopy);
            }
            riVM.OnRelationshipsMultiple = relOC;

            relOC = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in this.OnRelationshipsSingle)
            {
                var relCopy = rel.CreateCopy();
                relOC.Add(relCopy);
            }
            riVM.OnRelationshipsSingle = relOC;

            relOC = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in this.OnRelationshipsNone)
            {
                var relCopy = rel.CreateCopy();
                relOC.Add(relCopy);
            }
            riVM.OnRelationshipsNone = relOC;

            relOC = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in this.OtherRelationshipsMultiple)
            {
                var relCopy = rel.CreateCopy();
                relOC.Add(relCopy);
            }
            riVM.OtherRelationshipsMultiple = relOC;

            relOC = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in this.OtherRelationshipsSingle)
            {
                var relCopy = rel.CreateCopy();
                relOC.Add(relCopy);
            }
            riVM.OtherRelationshipsSingle = relOC;

            relOC = new ObservableCollection<RelationshipExtendedViewModel>();
            foreach (var rel in this.OtherRelationshipsNone)
            {
                var relCopy = rel.CreateCopy();
                relOC.Add(relCopy);
            }
            riVM.OtherRelationshipsNone = relOC;


            return riVM;
        }
    }
}
