
using NarrativeWorldCreator.MetricEngines;
using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class SelectedObjectDetailViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<EntikaInstanceValuedPredicate> _selectedInstancedEntikaInstances;
        public ObservableCollection<EntikaInstanceValuedPredicate> SelectedInstancedEntikaInstances
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

        internal void LoadSelectedInstances(List<EntikaInstance> selected, NarrativeTimePoint ntp)
        {
            ObservableCollection<EntikaInstanceValuedPredicate> eisVMoc = new ObservableCollection<EntikaInstanceValuedPredicate>();
            var listOfinstances = EntikaInstanceMetricEngine.GetDecorationOrderingEI(ntp);
            foreach (var instance in listOfinstances)
            {
                if (selected.Contains(instance.EntikaInstance))
                {
                    var eivp = new EntikaInstanceValuedPredicate(instance, ntp);
                    eisVMoc.Add(eivp);
                }
            }
            this.SelectedInstancedEntikaInstances = eisVMoc;
        }
    }
}
