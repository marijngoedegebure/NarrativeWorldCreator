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
        private ObservableCollection<GPUConfigurationResult> _GPUConfigurationResults;
        public ObservableCollection<GPUConfigurationResult> GPUConfigurationResults
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
            ObservableCollection<GPUConfigurationResult> gpcr = new ObservableCollection<GPUConfigurationResult>();
            foreach (var result in input)
            {
                gpcr.Add(result);
            }
            GPUConfigurationResults = gpcr;
        }
    }
}
