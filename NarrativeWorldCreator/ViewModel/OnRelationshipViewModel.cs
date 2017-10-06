﻿using NarrativeWorldCreator.Models.NarrativeRegionFill;
using Semantics.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    class OnRelationshipViewModel : INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        internal void Load(EntikaInstance instance, Relationship rel)
        {
            this.Source = instance;
            this.Relationship = rel;
            this.Required = false;
        }
    }
}