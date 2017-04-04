using NarrativeWorlds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    public class NarrativeEntitiesViewModel
    {
        private ObservableCollection<NarrativeEntityViewModel> _narrativeEntities;
        public ObservableCollection<NarrativeEntityViewModel> NarrativeEntities
        {
            get
            {
                return _narrativeEntities;
            }
            set
            {
                _narrativeEntities = value;
                OnPropertyChanged("NarrativeEntities");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(NarrativeTimePoint ntp, Node node)
        {

        }
    }
}
