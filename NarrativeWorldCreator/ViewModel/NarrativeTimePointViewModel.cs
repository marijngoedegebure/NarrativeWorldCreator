using NarrativeWorlds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    public class NarrativeTimePointViewModel : INotifyPropertyChanged
    {
        public NarrativeTimePoint NarrativeTimePoint { get; set; }
        public int TimePoint { get; set; }
        public string NarrativeActionName { get; set; }
        public string LocationName { get; set; }
        private bool _selected;
        public bool Selected {
            get { return _selected;  }
            set
            {
                this._selected = value;
                this.OnPropertyChanged("Selected");
            }
        }
        private bool _active;
        public bool Active
        {
            get { return _active;  }
            set
            {
                this._active = value;
                this.OnPropertyChanged("Active");
            }
        }

        public NarrativeTimePointViewModel(NarrativeTimePoint ntp)
        {
            NarrativeTimePoint = ntp;
            TimePoint = ntp.TimePoint;
            if (ntp.NarrativeEvent != null)
                NarrativeActionName = ntp.NarrativeEvent.NarrativeAction.Name;
            else
                NarrativeActionName = "";
            if (ntp.Location != null)
                LocationName = ntp.Location.LocationName;
            else
                LocationName = "";
            Selected = false;
            Active = true;
        }

        public NarrativeTimePointViewModel(NarrativeTimePoint ntp, bool active)
        {
            NarrativeTimePoint = ntp;
            TimePoint = ntp.TimePoint;
            if (ntp.NarrativeEvent != null)
                NarrativeActionName = ntp.NarrativeEvent.NarrativeAction.Name;
            else
                NarrativeActionName = "";
            if (ntp.Location != null)
                LocationName = ntp.Location.LocationName;
            else
                LocationName = "";
            Selected = false;
            Active = active;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
