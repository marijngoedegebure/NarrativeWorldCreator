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

        private ObservableCollection<NarrativeCharacter> _narrativeCharactersForSelectedNode;
        public ObservableCollection<NarrativeCharacter> NarrativeCharactersForSelectedNode
        {
            get
            {
                return _narrativeCharactersForSelectedNode;
            }
            set
            {
                _narrativeCharactersForSelectedNode = value;
                OnPropertyChanged("NarrativeCharactersForSelectedNode");
            }
        }

        private ObservableCollection<NarrativeThing> _narrativeThingsForSelectedNode;
        public ObservableCollection<NarrativeThing> NarrativeThingsForSelectedNode
        {
            get
            {
                return _narrativeThingsForSelectedNode;
            }
            set
            {
                _narrativeThingsForSelectedNode = value;
                OnPropertyChanged("NarrativeThingsForSelectedNode");
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

            foreach(NarrativeCharacter nc in ntp.GetNarrativeCharactersByNode(selectedNode))
            {
                ocnc.Add(nc);
            }
            foreach(NarrativeThing nt in ntp.GetNarrativeThingsByNode(selectedNode))
            {
                ocnt.Add(nt);
            }
            this.NarrativeCharactersForSelectedNode = ocnc;
            this.NarrativeThingsForSelectedNode = ocnt;
        }
    }
}
