using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Solvers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class RelationshipInstanceEnergyViewModel : INotifyPropertyChanged
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


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(RelationshipInstance relationship)
        {
            this.RelationshipInstance = relationship;
        }
    }
}
