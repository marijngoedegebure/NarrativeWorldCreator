using NarrativeWorldCreator.Models.NarrativeGraph;
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
    public class GraphDetailTimePointListViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<DetailTimePointViewModel> _narrativeTimePoints;
        public ObservableCollection<DetailTimePointViewModel> NarrativeTimePoints
        {
            get
            {
                return _narrativeTimePoints;
            }
            set
            {
                _narrativeTimePoints = value;
                OnPropertyChanged("NarrativeTimePoints");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void LoadTimePoints(LocationNode selectedNode)
        {
            ObservableCollection<DetailTimePointViewModel> ntpsVM = new ObservableCollection<DetailTimePointViewModel>();
            List<NarrativeTimePoint> ntps = SystemStateTracker.NarrativeWorld.NarrativeTimeline.getNarrativeTimePointsWithNode(selectedNode);
            // Load timepoint viewmodels based on narrative timepoints
            foreach (NarrativeTimePoint ntp in ntps)
            {
                var rdtpvm = new DetailTimePointViewModel();
                rdtpvm.LoadObjects(selectedNode, ntp);
                ntpsVM.Add(rdtpvm);
            }
            NarrativeTimePoints = ntpsVM;
        }
    }
}
