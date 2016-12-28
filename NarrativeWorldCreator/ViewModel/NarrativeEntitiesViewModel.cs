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
            var ncs = ntp.GetNarrativeCharactersByNode(node);
            var nts = ntp.GetNarrativeThingsByNode(node);
            var ocne = new ObservableCollection<NarrativeEntityViewModel>();

            foreach (var nc in ncs)
            {
                var nevm = new NarrativeEntityViewModel();
                nevm.Load(nc);
                ocne.Add(nevm);
            }

            foreach (var nt in nts)
            {
                var nevm = new NarrativeEntityViewModel();
                nevm.Load(nt);
                ocne.Add(nevm);
            }
            NarrativeEntities = ocne;
        }
    }
}
