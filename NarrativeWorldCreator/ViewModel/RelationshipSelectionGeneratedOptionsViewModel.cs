using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.ViewModel
{
    public class RelationshipSelectionGeneratedOptionsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<RelationshipSelectionAndInstancingViewModel> _generatedOptions;
        public ObservableCollection<RelationshipSelectionAndInstancingViewModel> GeneratedOptions
        {
            get
            {
                return _generatedOptions;
            }
            set
            {
                _generatedOptions = value;
                OnPropertyChanged("GeneratedOptions");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void Load(List<RelationshipSelectionAndInstancingViewModel> riVMs)
        {
            var optionsOC = new ObservableCollection<RelationshipSelectionAndInstancingViewModel>();
            foreach (var riVM in riVMs)
            {
                optionsOC.Add(riVM);
            }
            GeneratedOptions = optionsOC;
        }
    }
}
