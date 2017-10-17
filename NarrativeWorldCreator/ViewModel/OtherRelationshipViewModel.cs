using NarrativeWorldCreator.Models.NarrativeRegionFill;
using Semantics.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    class OtherRelationshipViewModel : INotifyPropertyChanged
    {

        private EntikaInstance _subjectInstance;
        public EntikaInstance SubjectInstance
        {
            get
            {
                return _subjectInstance;
            }
            set
            {
                _subjectInstance = value;
                OnPropertyChanged("SubjectInstance");
            }
        }

        private string _textblockText;
        public string TextblockText
        {
            get
            {
                return _textblockText;
            }
            set
            {
                _textblockText = value;
                OnPropertyChanged("TextblockText");
            }
        }

        private bool _asTarget;
        public bool AsTarget
        {
            get
            {
                return _asTarget;
            }
            set
            {
                _asTarget = value;
                OnPropertyChanged("AsTarget");
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

        internal void Load(EntikaInstance instance, Relationship rel, bool asTarget)
        {
            this.SubjectInstance = instance;
            this.Relationship = rel;
            this.AsTarget = asTarget;
            if (asTarget)
            {
                this.TextblockText = this.Relationship.RelationshipType.DefaultName + " (as target)" + " with " + this.SubjectInstance.Name;
            }
            else
            {
                this.TextblockText = this.Relationship.RelationshipType.DefaultName + "(as source)" + " with " + this.SubjectInstance.Name;
            }
            this.Required = false;
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            OtherRelationshipViewModel orVM = (OtherRelationshipViewModel)obj;
            if (!this.SubjectInstance.Equals(orVM.SubjectInstance))
                return false;
            if (!this.TextblockText.Equals(orVM.TextblockText))
                return false;
            if (!this.AsTarget.Equals(orVM.AsTarget))
                return false;
            if (!this.Relationship.Equals(orVM.Relationship))
                return false;
            if (!this.Required.Equals(orVM.Required))
                return false;
            return true;
        }
    }
}
