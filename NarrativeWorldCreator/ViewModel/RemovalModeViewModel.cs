using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    class RemovalModeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<EntikaInstance> _selectedInstancedEntikaInstances;
        public ObservableCollection<EntikaInstance> SelectedInstancedEntikaInstances
        {
            get
            {
                return _selectedInstancedEntikaInstances;
            }
            set
            {
                _selectedInstancedEntikaInstances = value;
                OnPropertyChanged("SelectedInstancedEntikaInstances");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        internal void LoadSelectedInstances(List<EntikaInstance> selected)
        {
            ObservableCollection<EntikaInstance> eisVMoc = new ObservableCollection<EntikaInstance>();
            foreach (var instance in selected)
            {
                    eisVMoc.Add(instance);
            }
            this.SelectedInstancedEntikaInstances = eisVMoc;
        }
    }
}
