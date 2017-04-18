using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class EntikaInstancesSelectionViewModel : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(NarrativeTimePoint ntp)
        {
            ObservableCollection<EntikaInstance> eioc = new ObservableCollection<EntikaInstance>();
            foreach (var instance in ntp.GetEntikaInstancesWithoutFloor())
            {
                eioc.Add(instance);
            }
            EntikaInstances = eioc;
        }
    }
}
