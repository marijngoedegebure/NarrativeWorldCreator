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
    public class RelationshipSelectionViewModel
    {
        private ObservableCollection<Relationship> _relationships;
        public ObservableCollection<Relationship> Relationships
        {
            get
            {
                return _relationships;
            }
            set
            {
                _relationships = value;
                OnPropertyChanged("Relationships");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(TangibleObject to)
        {
            ObservableCollection<Relationship> rsoc = new ObservableCollection<Relationship>();
            //foreach (var rel in to.RelationshipsAsSource)
            //{
            //    rsoc.Add(rel);
            //}
            foreach (var rel in to.RelationshipsAsTarget)
            {
                rsoc.Add(rel);
            }
            Relationships = rsoc;
        }
    }
}
