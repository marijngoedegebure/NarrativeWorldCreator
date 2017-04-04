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
    public class RegionDetailTimePointViewModel : INotifyPropertyChanged
    {

        private NarrativeTimePoint _narrativeTimePoint;
        public NarrativeTimePoint NarrativeTimePoint
        {
            get
            {
                return _narrativeTimePoint;
            }
            set
            {
                _narrativeTimePoint = value;
                OnPropertyChanged("NarrativeTimePoint");
            }
        }

        private ObservableCollection<NarrativeObjectEntikaLink> _narrativeObjectEntikaLinks;
        public ObservableCollection<NarrativeObjectEntikaLink> NarrativeObjectEntikaLinks
        {
            get
            {
                return _narrativeObjectEntikaLinks;
            }
            set
            {
                _narrativeObjectEntikaLinks = value;
                OnPropertyChanged("NarrativeObjectEntikaLinks");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        internal void LoadObjects(Node selectedNode, NarrativeTimePoint ntp)
        {
            this.NarrativeTimePoint = ntp;
            ObservableCollection<NarrativeObjectEntikaLink> nooc = new ObservableCollection<NarrativeObjectEntikaLink>();

            foreach(NarrativeObjectEntikaLink no in ntp.NarrativeObjectEntikaLinks)
            {
                nooc.Add(no);
            }
            this.NarrativeObjectEntikaLinks = nooc;
        }
    }
}
