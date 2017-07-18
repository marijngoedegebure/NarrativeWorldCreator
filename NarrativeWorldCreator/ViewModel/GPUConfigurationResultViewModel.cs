using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class GPUConfigurationResultViewModel : INotifyPropertyChanged
    {
        private GPUConfigurationResult _GPUConfigurationResult;
        public GPUConfigurationResult GPUConfigurationResult
        {
            get
            {
                return _GPUConfigurationResult;
            }
            set
            {
                _GPUConfigurationResult = value;
                OnPropertyChanged("GPUConfigurationResult");
            }
        }

        private float _totalCosts;
        public float TotalCosts
        {
            get
            {
                return _totalCosts;
            }
            set
            {
                _totalCosts = value;
                OnPropertyChanged("TotalCosts");
            }
        }

        private float _pairWiseCosts;
        public float PairWiseCosts
        {
            get
            {
                return _pairWiseCosts;
            }
            set
            {
                _pairWiseCosts = value;
                OnPropertyChanged("PairWiseCosts");
            }
        }

        private float _visualBalanceCosts;
        public float VisualBalanceCosts
        {
            get
            {
                return _visualBalanceCosts;
            }
            set
            {
                _visualBalanceCosts = value;
                OnPropertyChanged("VisualBalanceCosts");
            }
        }

        private float _focalPointCosts;
        public float FocalPointCosts
        {
            get
            {
                return _focalPointCosts;
            }
            set
            {
                _focalPointCosts = value;
                OnPropertyChanged("FocalPointCosts");
            }
        }

        private float _symmetryCosts;
        public float SymmetryCosts
        {
            get
            {
                return _symmetryCosts;
            }
            set
            {
                _symmetryCosts = value;
                OnPropertyChanged("SymmetryCosts");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        public void Load(GPUConfigurationResult input)
        {
            this.GPUConfigurationResult = input;
            this.TotalCosts = input.TotalCosts;
            this.PairWiseCosts = input.PairWiseCosts;
            this.FocalPointCosts = input.FocalPointCosts;
            this.SymmetryCosts = input.SymmetryCosts;
            this.VisualBalanceCosts = input.VisualBalanceCosts;
        }
    }
}
