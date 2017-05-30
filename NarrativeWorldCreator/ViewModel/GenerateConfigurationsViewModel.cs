using NarrativeWorldCreator.Models.NarrativeRegionFill;
using SharpDX.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class GenerateConfigurationsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<GPUConfigurationResultViewModel> _GPUConfigurationResults;
        public ObservableCollection<GPUConfigurationResultViewModel> GPUConfigurationResults
        {
            get
            {
                return _GPUConfigurationResults;
            }
            set
            {
                _GPUConfigurationResults = value;
                OnPropertyChanged("GPUConfigurationResults");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(List<GPUConfigurationResult> input)
        {
            ObservableCollection<GPUConfigurationResultViewModel> gpcr = new ObservableCollection<GPUConfigurationResultViewModel>();
            foreach (var result in input)
            {
                var gpcrVM = new GPUConfigurationResultViewModel();
                gpcrVM.Load(result);
                gpcr.Add(gpcrVM);
            }
            GPUConfigurationResults = gpcr;
        }
    }
}
