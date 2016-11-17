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
    public class NarrativeTimelineViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NarrativeTimePointViewModel> NarrativeTimePoints
        {
            get;
            set;
        }

        public void LoadTimePoints()
        {
            ObservableCollection<NarrativeTimePointViewModel> ntpsVM = new ObservableCollection<NarrativeTimePointViewModel>();
            List<NarrativeTimePoint> ntps = (from a in SystemStateTracker.NarrativeWorld.NarrativeTimeline.NarrativeTimePoints orderby a.TimePoint select a).ToList();
            // Load timepoint viewmodels based on narrative timepoints
            foreach(NarrativeTimePoint ntp in ntps)
            {
                ntpsVM.Add(new NarrativeTimePointViewModel(ntp));
            }
            NarrativeTimePoints = ntpsVM;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
