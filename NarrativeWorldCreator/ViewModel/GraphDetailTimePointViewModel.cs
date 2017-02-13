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
    public class GraphDetailTimePointViewModel : INotifyPropertyChanged
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

        private int _narrativeCharactersForSelectedNodeCount;
        public int NarrativeCharactersForSelectedNodeCount
        {
            get
            {
                return _narrativeCharactersForSelectedNodeCount;
            }
            set
            {
                _narrativeCharactersForSelectedNodeCount = value;
                OnPropertyChanged("NarrativeCharactersForSelectedNodeCount");
            }
        }

        private int _narrativeThingsForSelectedNodeCount;
        public int NarrativeThingsForSelectedNodeCount
        {
            get
            {
                return _narrativeThingsForSelectedNodeCount;
            }
            set
            {
                _narrativeThingsForSelectedNodeCount = value;
                OnPropertyChanged("NarrativeThingsForSelectedNodeCount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        internal void LoadCharactersAndThings(Node selectedNode, NarrativeTimePoint ntp)
        {
            this.NarrativeTimePoint = ntp;
            ObservableCollection<NarrativeCharacter> ocnc = new ObservableCollection<NarrativeCharacter>();
            ObservableCollection<NarrativeThing> ocnt = new ObservableCollection<NarrativeThing>();

            foreach (NarrativeCharacter nc in ntp.GetNarrativeCharactersByNode(selectedNode))
            {
                ocnc.Add(nc);
            }
            foreach (NarrativeThing nt in ntp.GetNarrativeThingsByNode(selectedNode))
            {
                ocnt.Add(nt);
            }
            this.NarrativeCharactersForSelectedNodeCount = ocnc.Count;
            this.NarrativeThingsForSelectedNodeCount = ocnt.Count;
        }
    }
}
