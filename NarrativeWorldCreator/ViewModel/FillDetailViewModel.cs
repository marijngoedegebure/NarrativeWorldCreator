﻿using NarrativeWorlds;
using NarrativeWorlds.Models.NarrativeRegionFill;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class FillDetailViewModel
    {
        private ObservableCollection<EntikaInstance> _entikaInstances;
        public ObservableCollection<EntikaInstance> EntikaInstances
        {
            get
            {
                return _entikaInstances;
            }
            set
            {
                _entikaInstances = value;
                OnPropertyChanged("EntikaInstances");
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
            ObservableCollection<EntikaInstance> eioc = new ObservableCollection<EntikaInstance>();
            foreach (var instance in ntp.InstancedObjects)
            {
                eioc.Add(instance);
            }
            EntikaInstances = eioc;

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
