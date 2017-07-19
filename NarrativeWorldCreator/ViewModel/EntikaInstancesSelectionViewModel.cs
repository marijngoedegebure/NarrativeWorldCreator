using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System.Collections.ObjectModel;
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

        public void Load(Configuration c)
        {
            ObservableCollection<EntikaInstance> eioc = new ObservableCollection<EntikaInstance>();
            foreach (var instance in c.GetEntikaInstancesWithoutFloor())
            {
                eioc.Add(instance);
            }
            EntikaInstances = eioc;
        }
    }
}
